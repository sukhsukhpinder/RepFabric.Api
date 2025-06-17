namespace RepFabric.Api.Models.Common
{
    /// <summary>
    /// Represents configuration settings for file storage backends,
    /// including local, S3, and SharePoint options.
    /// </summary>
    public class StorageSettings
    {
        /// <summary>
        /// Gets or sets the storage type ("Local", "S3", or "SharePoint").
        /// </summary>
        public string StorageType { get; set; } = "Local";

        /// <summary>
        /// Gets or sets the local folder path for file storage (used if StorageType is "Local").
        /// </summary>
        public string? LocalFolder { get; set; } = "Templates";

        /// <summary>
        /// Gets or sets the S3 bucket name (used if StorageType is "S3").
        /// </summary>
        public string? S3Bucket { get; set; }

        /// <summary>
        /// Gets or sets the SharePoint site ID (used if StorageType is "SharePoint").
        /// </summary>
        public string SharePointSiteId { get; set; }

        /// <summary>
        /// Gets or sets the SharePoint document library name (used if StorageType is "SharePoint").
        /// </summary>
        public string SharePointDocumentLibrary { get; set; }

        /// <summary>
        /// Gets or sets the SharePoint client ID for authentication.
        /// </summary>
        public string SharePointClientId { get; set; }

        /// <summary>
        /// Gets or sets the SharePoint tenant ID for authentication.
        /// </summary>
        public string SharePointTenantId { get; set; }

        /// <summary>
        /// Gets or sets the SharePoint client secret for authentication.
        /// </summary>
        public string SharePointClientSecret { get; set; }
    }
}