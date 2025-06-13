using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Models.Common;
using StorageSettings = RepFabric.Api.Models.Common.StorageSettings;

public class SharePointFileStorageService : IFileStorageService
{
    private readonly GraphServiceClient _graphClient;
    private readonly string _siteId;
    private readonly string _documentLibrary;
    private readonly ILogger<SharePointFileStorageService> _logger;

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
