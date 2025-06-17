using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Models.Common;

namespace RepFabric.Api.BL.Services
{
    /// <summary>
    /// Provides file storage operations using Amazon S3.
    /// Implements <see cref="IFileStorageService"/> for saving and retrieving files
    /// from a configured S3 bucket.
    /// </summary>
    public class S3FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3;
        private readonly string _bucket;
        private readonly ILogger<S3FileStorageService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="S3FileStorageService"/> class.
        /// </summary>
        /// <param name="s3">The Amazon S3 client instance.</param>
        /// <param name="options">The storage settings, including the S3 bucket name.</param>
        /// <param name="logger">The logger instance for logging operations.</param>
        public S3FileStorageService(IAmazonS3 s3, IOptions<StorageSettings> options, ILogger<S3FileStorageService> logger)
        {
            _s3 = s3;
            _bucket = options.Value.S3Bucket!;
            _logger = logger;
        }

        /// <summary>
        /// Saves a file to the configured S3 bucket.
        /// Throws an exception if the file already exists.
        /// </summary>
        /// <param name="fileName">The name of the file to save.</param>
        /// <param name="fileStream">The stream containing the file data.</param>
        /// <exception cref="InvalidOperationException">Thrown if the file already exists in the bucket.</exception>
        public async Task SaveFileAsync(string fileName, Stream fileStream)
        {
            if (await FileExistsAsync(fileName))
            {
                _logger.LogWarning($"File {fileName} already exists in S3 bucket {_bucket}.");
                throw new InvalidOperationException($"File '{fileName}' already exists in bucket '{_bucket}'.");
            }
            var transfer = new TransferUtility(_s3);
            await transfer.UploadAsync(fileStream, _bucket, fileName);
            _logger.LogInformation($"Saved file {fileName} to S3 bucket {_bucket}.");
        }

        /// <summary>
        /// Retrieves a file as a stream from the configured S3 bucket.
        /// </summary>
        /// <param name="fileName">The name of the file to retrieve.</param>
        /// <returns>A stream containing the file data.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist in the bucket.</exception>
        public async Task<Stream> GetFileAsync(string fileName)
        {
            try
            {
                var response = await _s3.GetObjectAsync(_bucket, fileName);
                _logger.LogInformation($"Retrieved file {fileName} from S3 bucket {_bucket}.");
                return response.ResponseStream;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"File {fileName} not found in S3 bucket {_bucket}.");
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// Checks if a file exists in the S3 bucket.
        /// </summary>
        /// <param name="key">The key (file name) to check.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        private async Task<bool> FileExistsAsync(string key)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucket,
                    Key = key
                };

                var response = await _s3.GetObjectMetadataAsync(request);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    }
}