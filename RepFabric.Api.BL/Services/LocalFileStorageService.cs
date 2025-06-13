using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Models.Common;

namespace RepFabric.Api.BL.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _folder;
        private readonly ILogger<LocalFileStorageService> _logger;

        public LocalFileStorageService(IOptions<StorageSettings> options, ILogger<LocalFileStorageService> logger)
        {
            _folder = options.Value.LocalFolder ?? "Templates";
            _logger = logger;
        }

        public async Task SaveFileAsync(string fileName, Stream fileStream)
        {
            var path = Path.Combine(_folder, fileName);
            Directory.CreateDirectory(_folder);
            using var file = File.Create(path);
            await fileStream.CopyToAsync(file);
            _logger.LogInformation($"Saved file {fileName} to local folder {_folder}.");
        }

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