using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepFabric.Api.BL.Interfaces
{
    /// <summary>
    /// Defines methods for synchronizing and retrieving data from Yoxel endpoints.
    /// Provides a generic asynchronous GET operation with support for route parameters and optional basic authentication.
    /// </summary>
    public interface IYoxelSyncService
    {
        /// <summary>
        /// Sends a GET request to the specified Yoxel endpoint and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to.</typeparam>
        /// <param name="baseUrl">The base URL of the Yoxel API.</param>
        /// <param name="route">The route or endpoint to call, with optional placeholders for route parameters.</param>
        /// <param name="routeParams">Optional dictionary of route parameters to replace in the route.</param>
        /// <param name="basicAuth">Optional basic authentication token.</param>
        /// <returns>A task that returns the deserialized response object, or null if the response is empty.</returns>
        Task<T?> GetAsync<T>(string baseUrl, string route, IDictionary<string, string>? routeParams = null, string? basicAuth = null);
    }
}
