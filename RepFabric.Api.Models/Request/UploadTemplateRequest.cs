using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RepFabric.Api.Models.Request
{
    public class UploadTemplateRequest
    {
        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; } = default!;
    }
}