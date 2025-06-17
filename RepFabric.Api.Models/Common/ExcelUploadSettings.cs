namespace RepFabric.Api.Models.Common
{
    /// <summary>
    /// Represents configuration settings for Excel file uploads,
    /// including allowed file extensions, MIME types, and maximum file size.
    /// </summary>
    public class ExcelUploadSettings
    {
        /// <summary>
        /// Gets or sets the allowed file extensions for Excel uploads (e.g., ".xlsx", ".xls").
        /// </summary>
        public string[] AllowedExtensions { get; set; } = [];

        /// <summary>
        /// Gets or sets the allowed MIME types for Excel uploads.
        /// </summary>
        public string[] AllowedMimeTypes { get; set; } = [];

        /// <summary>
        /// Gets or sets the maximum allowed file size for uploads, in bytes.
        /// </summary>
        public long MaxFileSize { get; set; }
    }
}
