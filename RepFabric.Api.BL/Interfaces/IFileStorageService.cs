namespace RepFabric.Api.BL.Interfaces
{
    /// <summary>
    /// Defines methods for file storage operations such as saving and retrieving files.
    /// Implementations can provide storage using local file system, cloud storage, or other backends.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Saves a file to the storage backend.
        /// </summary>
        /// <param name="fileName">The name of the file to save.</param>
        /// <param name="fileStream">The stream containing the file data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveFileAsync(string fileName, Stream fileStream);

        /// <summary>
        /// Retrieves a file as a stream from the storage backend.
        /// </summary>
        /// <param name="fileName">The name of the file to retrieve.</param>
        /// <returns>A task that returns a stream containing the file data.</returns>
        Task<Stream> GetFileAsync(string fileName);
    }
}