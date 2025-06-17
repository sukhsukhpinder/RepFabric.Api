using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RepFabric.Api.Models.Request
{
    /// <summary>
    /// Represents a request to upload a template file.
    /// Contains the file to be uploaded as part of the request.
    /// </summary>
    public class UploadTemplateRequest
    {
        /// <summary>
        /// Gets or sets the template file to upload.
        /// </summary>
        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; } = default!;
    }
}