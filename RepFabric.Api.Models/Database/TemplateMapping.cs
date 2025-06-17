using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepFabric.Api.Models.Database
{
    public class TemplateMapping
    {
        [Key]
        public int Id { get; set; }
        public string TemplateFileName { get; set; }
        public string MappingJson { get; set; } // JSON string representing the mapping details
    }
}
