namespace RepFabric.Api.BL.Enums
{
    /// <summary>
    /// Specifies the available storage backends for file operations in the application.
    /// </summary>
    public enum StorageTypes
    {
        /// <summary>
        /// Use Amazon S3 as the storage backend.
        /// </summary>
        S3,

        /// <summary>
        /// Use the local file system as the storage backend.
        /// </summary>
        Local,

        /// <summary>
        /// Use Microsoft SharePoint as the storage backend.
        /// </summary>
        Sharepoint
    }
}
