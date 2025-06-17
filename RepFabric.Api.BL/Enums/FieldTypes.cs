namespace RepFabric.Api.BL.Enums
{
    /// <summary>
    /// Specifies the types of fields that can be mapped in an Excel template.
    /// </summary>
    public enum FieldTypes
    {
        /// <summary>
        /// Represents a simple text field mapping.
        /// </summary>
        Text,

        /// <summary>
        /// Represents a line item field, typically used for tabular data.
        /// </summary>
        LineItem,

        /// <summary>
        /// Represents a dropdown field, allowing selection from predefined options.
        /// </summary>
        Dropdown
    }
}
