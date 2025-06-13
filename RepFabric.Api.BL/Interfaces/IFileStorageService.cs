namespace RepFabric.Api.BL.Interfaces
{
    public interface IFileStorageService
    {
        Task SaveFileAsync(string fileName, Stream fileStream);
        Task<Stream> GetFileAsync(string fileName);
    }
}