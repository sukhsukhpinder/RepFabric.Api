

using RepFabric.Api.Models.Request;

namespace RepFabric.Api.BL.Interfaces
{
    public interface IExcelTemplateService
    {
        Task<byte[]> FillTemplateAsync(string templateFileName, ExcelMappingRequest request);
        Task<IEnumerable<string>> ListTemplatesAsync();
    }
}