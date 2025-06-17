using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Models.Common;

namespace RepFabric.Api.BL.Services
{
    /// <summary>
    /// Provides file storage operations using the local file system.
    /// Implements <see cref="IFileStorageService"/> for saving and retrieving files
    /// from a configurable local folder.
    /// </summary>
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _folder;
        private readonly ILogger<LocalFileStorageService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileStorageService"/> class.
        /// </summary>
        /// <param name="options">The storage settings, including the local folder path.</param>
        /// <param name="logger">The logger instance for logging operations.</param>
        public LocalFileStorageService(IOptions<StorageSettings> options, ILogger<LocalFileStorageService> logger)
        {
            _folder = options.Value.LocalFolder ?? "Templates";
            _logger = logger;
        }

        /// <summary>
        /// Saves a file to the local storage folder.
        /// </summary>
        /// <param name="fileName">The name of the file to save.</param>
        /// <param name="fileStream">The stream containing the file data.</param>
        public async Task SaveFileAsync(string fileName, Stream fileStream)
        {
            var path = Path.Combine(_folder, fileName);
            Directory.CreateDirectory(_folder);
            using var file = File.Create(path);
            await fileStream.CopyToAsync(file);
            _logger.LogInformation($"Saved file {fileName} to local folder {_folder}.");
        }

        /// <summary>
        /// Retrieves a file as a stream from the local storage folder.
        /// </summary>
        /// <param name="fileName">The name of the file to retrieve.</param>
        /// <returns>A stream containing the file data.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        public Task<Stream> GetFileAsync(string fileName)
        {
            var path = Path.Combine(_folder, fileName);
            if (!File.Exists(path))
            {
                _logger.LogWarning($"File {fileName} not found in local folder {_folder}.");
                throw new FileNotFoundException();
            }
            _logger.LogInformation($"Retrieved file {fileName} from local folder {_folder}.");
            return Task.FromResult<Stream>(File.OpenRead(path));
        }
    }
}
