using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Models.Common;

namespace RepFabric.Api.BL.Services
{
    /// <summary>
    /// Provides file storage operations using Microsoft SharePoint via Microsoft Graph API.
    /// Implements <see cref="IFileStorageService"/> for saving and retrieving files
    /// from a configured SharePoint document library.
    /// </summary>
    public class SharePointFileStorageService : IFileStorageService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly string _siteId;
        private readonly string _documentLibrary;
        private readonly ILogger<SharePointFileStorageService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointFileStorageService"/> class.
        /// </summary>
        /// <param name="options">The storage settings, including SharePoint site and library information.</param>
        /// <param name="logger">The logger instance for logging operations.</param>
        public SharePointFileStorageService(
            IOptions<StorageSettings> options,
            ILogger<SharePointFileStorageService> logger)
        {
            _siteId = options.Value.SharePointSiteId;
            _documentLibrary = options.Value.SharePointDocumentLibrary;
            _logger = logger;

            var credential = new ClientSecretCredential(
                options.Value.SharePointTenantId,
                options.Value.SharePointClientId,
                options.Value.SharePointClientSecret);

            _graphClient = new GraphServiceClient(credential);
        }

        /// <summary>
        /// Saves a file to the configured SharePoint document library.
        /// </summary>
        /// <param name="fileName">The name of the file to save.</param>
        /// <param name="fileStream">The stream containing the file data.</param>
        /// <exception cref="InvalidOperationException">Thrown if the document library is not found.</exception>
        /// <exception cref="Exception">Logs and rethrows any error that occurs during the upload process.</exception>
        public async Task SaveFileAsync(string fileName, Stream fileStream)
        {
            try
            {
                var drives = await _graphClient.Sites[_siteId].Drives.GetAsync();
                var drive = drives.Value?.FirstOrDefault(d => d.Name == _documentLibrary);

                if (drive == null)
                    throw new InvalidOperationException($"Document library '{_documentLibrary}' not found.");

                await _graphClient.Drives[drive.Id]
                    .Root
                    .ItemWithPath($"{_documentLibrary}/{fileName}")
                    .Content
                    .PutAsync(fileStream);

                _logger.LogInformation("Saved file {FileName} to SharePoint library {Library}.", fileName, _documentLibrary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file {FileName} to SharePoint.", fileName);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a file as a stream from the configured SharePoint document library.
        /// </summary>
        /// <param name="fileName">The name of the file to retrieve.</param>
        /// <returns>A stream containing the file data.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist in the library.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the document library is not found.</exception>
        /// <exception cref="Exception">Logs and rethrows any error that occurs during the retrieval process.</exception>
        public async Task<Stream> GetFileAsync(string fileName)
        {
            try
            {
                var drives = await _graphClient.Sites[_siteId].Drives.GetAsync();
                var drive = drives.Value?.FirstOrDefault(d => d.Name == _documentLibrary);

                if (drive == null)
                    throw new InvalidOperationException($"Document library '{_documentLibrary}' not found.");

                var stream = await _graphClient.Drives[drive.Id]
                    .Root
                    .ItemWithPath(fileName)
                    .Content
                    .GetAsync();

                _logger.LogInformation("Retrieved file {FileName} from SharePoint library {Library}.", fileName, _documentLibrary);
                return stream;
            }
            catch (ServiceException ex) when (ex.ResponseStatusCode == 404)
            {
                _logger.LogWarning("File {FileName} not found in SharePoint library {Library}.", fileName, _documentLibrary);
                throw new FileNotFoundException($"File '{fileName}' not found in SharePoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file {FileName} from SharePoint.", fileName);
                throw;
            }
        }
    }
}