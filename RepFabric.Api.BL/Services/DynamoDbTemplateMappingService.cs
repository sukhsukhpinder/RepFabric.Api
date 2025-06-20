using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Models.DynamoDb;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepFabric.Api.BL.Services
{
    /// <summary>
    /// Service for CRUD operations on TemplateMapping entities in DynamoDB.
    /// </summary>
    public class DynamoDbTemplateMappingService : IDynamoDbTemplateMappingService
    {

        private readonly IDynamoDBContext _dbContext;

        public DynamoDbTemplateMappingService(IAmazonDynamoDB dynamoDb)
        {
            _dbContext = new DynamoDBContext(dynamoDb);
        }
        /// <summary>
        /// Creates a new TemplateMapping in DynamoDB.
        /// </summary>
        public async Task CreateAsync(string templateFileName, string mappingJson)
        {
            await _dbContext.SaveAsync(new TemplateMapping()
            {
                TemplateFileName = templateFileName,
                MappingJson = mappingJson,
            });
        }

        /// <summary>
        /// Retrieves a TemplateMapping by Id.
        /// </summary>
        public async Task<TemplateMapping?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.LoadAsync<TemplateMapping>(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"TemplateMapping with id {id} does not exist.");
            }
            return await _dbContext.LoadAsync<TemplateMapping>(id);
        }

        /// <summary>
        /// Retrieves all TemplateMappings.
        /// </summary>
        public async Task<List<TemplateMapping>> GetAllAsync()
        {
            var conditions = new List<ScanCondition>();
            return await _dbContext.ScanAsync<TemplateMapping>(conditions).GetRemainingAsync();
        }

        /// <summary>
        /// Updates an existing TemplateMapping.
        /// </summary>
        public async Task UpdateAsync(int id, TemplateMapping mapping)
        {
            var existing = await _dbContext.LoadAsync<TemplateMapping>(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"TemplateMapping with id {id} does not exist.");
            }
            mapping.Id = id; // Ensure the Id is set for the update
            await _dbContext.SaveAsync(mapping);
        }

        /// <summary>
        /// Deletes a TemplateMapping by Id.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var existing = await _dbContext.LoadAsync<TemplateMapping>(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"TemplateMapping with id {id} does not exist.");
            }
            await _dbContext.DeleteAsync<TemplateMapping>(id);
        }
    }
}