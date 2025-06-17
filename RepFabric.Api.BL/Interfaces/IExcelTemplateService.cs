using RepFabric.Api.Models.Database;
using RepFabric.Api.Models.Request;

namespace RepFabric.Api.BL.Interfaces
{
    /// <summary>
    /// Provides methods for managing Excel templates and their mappings,
    /// including filling templates, listing available templates, and CRUD operations for template mappings.
    /// </summary>
    public interface IExcelTemplateService
    {
        /// <summary>
        /// Fills an Excel template with data for a given order using a specified mapping.
        /// </summary>
        /// <param name="orderId">The identifier of the order to use for populating the template.</param>
        /// <param name="mappingId">The identifier of the template mapping to apply.</param>
        /// <returns>A byte array representing the filled Excel file.</returns>
        Task<byte[]> FillTemplateAsync(string orderId, int mappingId);

        /// <summary>
        /// Lists the names of all available Excel templates.
        /// </summary>
        /// <returns>An enumerable collection of template file names.</returns>
        Task<IEnumerable<string>> ListTemplatesAsync();

        /// <summary>
        /// Retrieves all template mappings from the data store.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="TemplateMapping"/> objects.</returns>
        Task<IEnumerable<TemplateMapping>> GetTemplateMappingsAsync();

        /// <summary>
        /// Saves a new template mapping to the data store.
        /// </summary>
        /// <param name="templateFileName">The name of the template file.</param>
        /// <param name="mappingJson">The JSON string representing the mapping configuration.</param>
        Task SaveTemplateMappingAsync(string templateFileName, string mappingJson);

        /// <summary>
        /// Deletes a template mapping by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the template mapping to delete.</param>
        /// <returns>True if the mapping was deleted; otherwise, false.</returns>
        Task<bool> DeleteTemplateMappingAsync(int id);

        /// <summary>
        /// Updates an existing template mapping with new values.
        /// </summary>
        /// <param name="id">The identifier of the template mapping to update.</param>
        /// <param name="templateFileName">The new template file name.</param>
        /// <param name="mappingJson">The new mapping configuration as a JSON string.</param>
        /// <returns>True if the mapping was updated; otherwise, false.</returns>
        Task<bool> UpdateTemplateMappingAsync(int id, string templateFileName, string mappingJson);
    }
}