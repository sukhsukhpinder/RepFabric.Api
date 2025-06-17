using Microsoft.AspNetCore.Http;

namespace RepFabric.Api.BL.Validators
{
    /// <summary>
    /// Provides validation logic for uploaded files, including checks for allowed extensions,
    /// MIME types, and maximum file size.
    /// </summary>
    public class FileUploadValidator
    {
        private readonly string[] _allowedExtensions;
        private readonly string[] _allowedMimeTypes;
        private readonly long _maxFileSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadValidator"/> class.
        /// </summary>
        /// <param name="allowedExtensions">Array of allowed file extensions (e.g., ".xlsx", ".csv").</param>
        /// <param name="allowedMimeTypes">Array of allowed MIME types (e.g., "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet").</param>
        /// <param name="maxFileSize">Maximum allowed file size in bytes.</param>
        public FileUploadValidator(string[] allowedExtensions, string[] allowedMimeTypes, long maxFileSize)
        {
            _allowedExtensions = allowedExtensions;
            _allowedMimeTypes = allowedMimeTypes;
            _maxFileSize = maxFileSize;
        }

        /// <summary>
        /// Validates the specified uploaded file against extension, MIME type, and size constraints.
        /// </summary>
        /// <param name="file">The uploaded file to validate.</param>
        /// <returns>
        /// A tuple containing a boolean indicating validity and an error message if invalid.
        /// </returns>
        public (bool IsValid, string ErrorMessage) Validate(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "No file uploaded.");

            if (file.Length > _maxFileSize)
                return (false, $"File size exceeds the {_maxFileSize / (1024 * 1024)} MB limit.");

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(fileExtension))
                return (false, $"Only {string.Join(", ", _allowedExtensions)} files are allowed.");

            if (!_allowedMimeTypes.Contains(file.ContentType))
                return (false, "Invalid file type.");

            return (true, string.Empty);
        }
    }
}
