using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepFabric.Api.Models.Database
{
    /// <summary>
    /// Represents a mapping configuration for an Excel template.
    /// Stores the template file name and the mapping details as a JSON string.
    /// </summary>
    public class TemplateMapping
    {
        /// <summary>
        /// Gets or sets the unique identifier for the template mapping.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the file name of the associated Excel template.
        /// </summary>
        public string TemplateFileName { get; set; }

        /// <summary>
        /// Gets or sets the mapping details as a JSON string.
        /// This JSON defines how data fields are mapped to cells in the template.
        /// </summary>
        public string MappingJson { get; set; } // JSON string representing the mapping details
    }
}
