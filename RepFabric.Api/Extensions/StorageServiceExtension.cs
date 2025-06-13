using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepFabric.Api.BL.Enums;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.BL.Services;
using RepFabric.Api.Models.Common;

namespace RepFabric.Api.Extensions
{
    public static class StorageServiceExtension
    {
        public static IServiceCollection AddConfiguredFileStorageService(this IServiceCollection services)
        {
            services.AddTransient<IFileStorageService>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<StorageSettings>>().Value;
                if (settings.StorageType.Equals(nameof(StorageTypes.S3), StringComparison.OrdinalIgnoreCase))
                {
                    return new S3FileStorageService(
                        sp.GetRequiredService<IAmazonS3>(),
                        sp.GetRequiredService<IOptions<StorageSettings>>(),
                        sp.GetRequiredService<ILogger<S3FileStorageService>>());
                }
                else if (settings.StorageType.Equals(nameof(StorageTypes.Sharepoint), StringComparison.OrdinalIgnoreCase))
                {
                    // Placeholder for SharePoint storage service
                    // return new SharePointFileStorageService(
                    //     sp.GetRequiredService<IOptions<StorageSettings>>(),
                    //     sp.GetRequiredService<ILogger<SharePointFileStorageService>>());
                    throw new NotImplementedException("SharePoint storage service is not implemented yet.");
                }
                else
                {
                    return new LocalFileStorageService(
                        sp.GetRequiredService<IOptions<StorageSettings>>(),
                        sp.GetRequiredService<ILogger<LocalFileStorageService>>());
                }
            });

            return services;
        }
    }
}