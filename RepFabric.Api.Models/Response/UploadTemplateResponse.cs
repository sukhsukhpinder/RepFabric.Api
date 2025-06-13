using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepFabric.Api.Models.Response
{
    public class UploadTemplateResponse
    {
        public string Message { get; set; }
        public string FileName { get; set; }
        public string Storage { get; set; }
    }
}
