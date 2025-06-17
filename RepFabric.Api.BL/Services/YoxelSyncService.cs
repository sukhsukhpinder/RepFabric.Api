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
    public class YoxelSyncService : IYoxelSyncService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IYoxelSyncService> _logger;

        public YoxelSyncService(IHttpClientFactory httpClientFactory, ILogger<IYoxelSyncService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

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
