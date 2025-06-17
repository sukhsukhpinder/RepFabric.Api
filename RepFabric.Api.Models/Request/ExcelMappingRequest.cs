using System.ComponentModel.DataAnnotations;

namespace RepFabric.Api.Models.Request
{
    /// <summary>
    /// Represents a request to map data fields to an Excel template.
    /// Contains the template file name and a list of cell-to-field mappings.
    /// </summary>
    public class ExcelMappingRequest
    {
        /// <summary>
        /// Gets or sets the file name of the Excel template to be used.
        /// </summary>
        [Required(ErrorMessage = "TemplateFileName is required.")]
        public string TemplateFileName { get; set; }        

        /// <summary>
        /// Gets or sets the list of mappings that define how data fields are mapped to Excel cells.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "At least one mapping is required.")]
        public List<Mapping> Mappings { get; set; } = new();
    }

    /// <summary>
    /// Represents a mapping between a data field and an Excel cell.
    /// </summary>
    public class Mapping
    {
        /// <summary>
        /// Gets or sets the Excel cell reference (e.g., "A1", "B2") for this mapping.
        /// </summary>
        [Required]
        public string Cell { get; set; }

        /// <summary>
        /// Gets or sets the type of field being mapped (e.g., "Text", "LineItem", "Dropdown").
        /// </summary>
        [Required]
        public string FieldType { get; set; }

        /// <summary>
        /// Gets or sets the name of the attribute to be mapped to this cell.
        /// </summary>
        public string? AttributeName { get; set; }
    }
}