using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepFabric.Api.BL.Constants
{
    /// <summary>
    /// Contains constant values for Yoxel API endpoints used throughout the application.
    /// </summary>
    public static class YoxelConstants
    {
        /// <summary>
        /// The API route template for retrieving a purchase order from the Yoxel sync service.
        /// Replace <c>{orderId}</c> with the actual order identifier when making requests.
        /// </summary>
        public static string PurchaseOrder = "/yoxel_sync/v1.3/purchase_orders/{orderId}";
    }
}
