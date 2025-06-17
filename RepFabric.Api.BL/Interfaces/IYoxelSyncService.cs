using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepFabric.Api.BL.Interfaces
{
    public interface IYoxelSyncService
    {
        Task<T?> GetAsync<T>(string baseUrl, string route, IDictionary<string, string>? routeParams = null, string? basicAuth = null);
    }
}
