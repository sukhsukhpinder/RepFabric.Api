using System.ComponentModel.DataAnnotations;

namespace RepFabric.Api.Models.Request
{
    public class ExcelMappingRequest
    {
        [Required(ErrorMessage = "TemplateFileName is required.")]
        public string TemplateFileName { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one mapping is required.")]
        public List<Mapping> Mappings { get; set; } = new();
    }

    public class Mapping
    {
        [Required]
        public string Cell { get; set; }

        [Required]
        public string FieldType { get; set; }

        /// <summary>
        /// The name of the attribute to be mapped to this cell.
        /// </summary>
        public string? AttributeName { get; set; }
    }
}