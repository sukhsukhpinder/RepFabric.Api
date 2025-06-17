using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepFabric.Api.Models.Common
{
    /// <summary>
    /// Represents configuration settings for connecting to the Yoxel API.
    /// </summary>
    public class YoxelSettings
    {
        /// <summary>
        /// Gets or sets the base address of the Yoxel API.
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the authentication token used for Yoxel API requests.
        /// </summary>
        public string AuthToken { get; set; }
    }
}
