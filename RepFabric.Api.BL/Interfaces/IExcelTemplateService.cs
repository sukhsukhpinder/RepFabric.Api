

using RepFabric.Api.Models.Database;
using RepFabric.Api.Models.Request;

namespace RepFabric.Api.BL.Interfaces
{
    public interface IExcelTemplateService
    {
        Task<byte[]> FillTemplateAsync(string orderId, int mappingId);
        Task<IEnumerable<string>> ListTemplatesAsync();
        Task<IEnumerable<TemplateMapping>> GetTemplateMappingsAsync();
        Task SaveTemplateMappingAsync(string templateFileName, string mappingJson);
        Task<bool> DeleteTemplateMappingAsync(int id);
        Task<bool> UpdateTemplateMappingAsync(int id, string templateFileName, string mappingJson);
    }
}