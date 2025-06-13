using Microsoft.AspNetCore.Http;

namespace RepFabric.Api.BL.Validators
{
    public class FileUploadValidator
    {
        private readonly string[] _allowedExtensions;
        private readonly string[] _allowedMimeTypes;
        private readonly long _maxFileSize;

        public FileUploadValidator(string[] allowedExtensions, string[] allowedMimeTypes, long maxFileSize)
        {
            _allowedExtensions = allowedExtensions;
            _allowedMimeTypes = allowedMimeTypes;
            _maxFileSize = maxFileSize;
        }

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
