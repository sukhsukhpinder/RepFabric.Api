namespace RepFabric.Api.Models.Common
{
    public class ExcelUploadSettings
    {
        public string[] AllowedExtensions { get; set; } = [];
        public string[] AllowedMimeTypes { get; set; } = [];
        public long MaxFileSize { get; set; }
    }
}
