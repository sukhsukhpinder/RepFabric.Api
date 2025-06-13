using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Models.Common;

namespace RepFabric.Api.BL.Services
{
    public class S3FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3;
        private readonly string _bucket;
        private readonly ILogger<S3FileStorageService> _logger;

        public S3FileStorageService(IAmazonS3 s3, IOptions<StorageSettings> options, ILogger<S3FileStorageService> logger)
        {
            _s3 = s3;
            _bucket = options.Value.S3Bucket!;
            _logger = logger;
        }

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