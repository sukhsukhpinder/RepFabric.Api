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
    /// <summary>
    /// Provides extension methods for configuring file storage services in the application's dependency injection container.
    /// </summary>
    public static class StorageServiceExtension
    {
        /// <summary>
        /// Registers an <see cref="IFileStorageService"/> implementation with the dependency injection container
        /// based on the configured <see cref="StorageSettings.StorageType"/>.
        /// </summary>
        /// <param name="services">The service collection to add the storage service to.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        /// <remarks>
        /// - If <c>StorageType</c> is "S3", registers <see cref="S3FileStorageService"/>.
        /// - If <c>StorageType</c> is "Sharepoint", throws <see cref="NotImplementedException"/> (placeholder for future implementation).
        /// - Otherwise, registers <see cref="LocalFileStorageService"/> as the default.
        /// </remarks>
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