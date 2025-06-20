
using RepFabric.Api.Models.DynamoDb;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepFabric.Api.BL.Interfaces
{
    /// <summary>
    /// Defines CRUD operations for TemplateMapping entities in DynamoDB.
    /// </summary>
    public interface IDynamoDbTemplateMappingService
    {
        /// <summary>
        /// Creates a new TemplateMapping in DynamoDB.
        /// </summary>
        Task CreateAsync(string TemplateFileName, string mappingJson);

        /// <summary>
        /// Retrieves a TemplateMapping by its Id.
        /// </summary>
        Task<TemplateMapping?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all TemplateMappings.
        /// </summary>
        Task<List<TemplateMapping>> GetAllAsync();

        /// <summary>
        /// Updates an existing TemplateMapping.
        /// </summary>
        Task UpdateAsync(int id,TemplateMapping mapping);

        /// <summary>
        /// Deletes a TemplateMapping by its Id.
        /// </summary>
        Task DeleteAsync(int id);
    }
}