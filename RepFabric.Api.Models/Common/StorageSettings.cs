namespace RepFabric.Api.Models.Common
{
    public class StorageSettings
    {
        public string StorageType { get; set; } = "Local"; // "Local" or "S3" or "SharePoint"
        public string? LocalFolder { get; set; } = "Templates";
        public string? S3Bucket { get; set; }
        public string SharePointSiteId { get; set; }
        public string SharePointDocumentLibrary { get; set; }
        public string SharePointClientId { get; set; }
        public string SharePointTenantId { get; set; }
        public string SharePointClientSecret { get; set; }
    }
}