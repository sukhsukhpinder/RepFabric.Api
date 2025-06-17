using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using RepFabric.Api.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RepFabric.Api.BL.Services
{
    /// <summary>
    /// Service for synchronizing data with Yoxel endpoints using HTTP requests.
    /// Implements <see cref="IYoxelSyncService"/> to provide generic GET operations
    /// with optional route parameters and basic authentication.
    /// </summary>
    public class YoxelSyncService : IYoxelSyncService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IYoxelSyncService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="YoxelSyncService"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Factory for creating HTTP client instances.</param>
        /// <param name="logger">Logger for logging request information and errors.</param>
        public YoxelSyncService(IHttpClientFactory httpClientFactory, ILogger<IYoxelSyncService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Sends a GET request to the specified Yoxel endpoint and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to.</typeparam>
        /// <param name="baseUrl">The base URL of the Yoxel API.</param>
        /// <param name="route">The route or endpoint to call, with optional placeholders for route parameters.</param>
        /// <param name="routeParams">Optional dictionary of route parameters to replace in the route.</param>
        /// <param name="basicAuth">Optional basic authentication token.</param>
        /// <returns>The deserialized response object, or null if the response is empty.</returns>
        public async Task<T?> GetAsync<T>(string baseUrl, string route, IDictionary<string, string>? routeParams = null, string? basicAuth = null)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            // Build route with params
            if (routeParams != null)
            {
                foreach (var kvp in routeParams)
                {
                    route = route.Replace($"{{{kvp.Key}}}", kvp.Value);
                }
            }

            var request = new HttpRequestMessage(HttpMethod.Get, route);

            if (!string.IsNullOrEmpty(basicAuth))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            }

            _logger.LogInformation("Sending GET request to {Url}", client.BaseAddress + route);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }
}
