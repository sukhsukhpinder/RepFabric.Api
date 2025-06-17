using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepFabric.Api.Models.Response
{
    /// <summary>
    /// Represents the response returned after uploading a template file.
    /// Contains information about the upload result, file name, and storage location.
    /// </summary>
    public class UploadTemplateResponse
    {
        /// <summary>
        /// Gets or sets the message describing the result of the upload operation.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the name of the uploaded file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the storage backend where the file was saved (e.g., "Local", "S3", "SharePoint").
        /// </summary>
        public string Storage { get; set; }
    }
}
