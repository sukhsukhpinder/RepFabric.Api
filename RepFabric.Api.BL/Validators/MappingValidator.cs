using RepFabric.Api.BL.Enums;
using RepFabric.Api.Models.Request;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RepFabric.Api.BL.Validators
{
    /// <summary>
    /// Provides validation logic for <see cref="Mapping"/> objects used in Excel template mapping.
    /// Ensures that each mapping has a valid cell reference, field type, and required attributes
    /// based on the field type.
    /// </summary>
    public static class MappingValidator
    {
        // List of allowed field types for mapping validation.
        private static readonly string[] AllowedFieldTypes = { nameof(FieldTypes.Text), nameof(FieldTypes.Dropdown), nameof(FieldTypes.LineItem) }; // Extend as needed

        /// <summary>
        /// Validates a collection of <see cref="Mapping"/> objects and returns all validation results.
        /// </summary>
        /// <param name="mappings">The collection of mappings to validate.</param>
        /// <returns>An enumerable of <see cref="ValidationResult"/> for any validation errors found.</returns>
        public static IEnumerable<ValidationResult> ValidateAll(IEnumerable<Mapping> mappings)
        {
            return mappings
                .SelectMany((mapping, i) => Validate(mapping, i))
                .ToList();
        }

        /// <summary>
        /// Validates a single <see cref="Mapping"/> object and returns any validation errors.
        /// </summary>
        /// <param name="mapping">The mapping to validate.</param>
        /// <param name="index">The index of the mapping in the collection (for error reporting).</param>
        /// <returns>An enumerable of <see cref="ValidationResult"/> for any validation errors found.</returns>
        public static IEnumerable<ValidationResult> Validate(Mapping mapping, int index)
        {
            // Validate Cell: must match Excel cell reference (e.g., A1, B2, AA10)
            if (string.IsNullOrWhiteSpace(mapping.Cell) || !Regex.IsMatch(mapping.Cell, @"^[A-Z]+[1-9][0-9]*$", RegexOptions.IgnoreCase))
            {
                yield return new ValidationResult(
                    $"Mapping[{index}]: Cell must be a valid Excel cell reference (e.g., A1, B2).",
                    new[] { $"Mappings[{index}].Cell" });
            }

            // Validate FieldType: must be one of the allowed types
            if (string.IsNullOrWhiteSpace(mapping.FieldType) || !AllowedFieldTypes.Contains(mapping.FieldType, StringComparer.OrdinalIgnoreCase))
            {
                yield return new ValidationResult(
                    $"Mapping[{index}]: FieldType must be one of: {string.Join(", ", AllowedFieldTypes)}.",
                    new[] { $"Mappings[{index}].FieldType" });
            }

            // If FieldType is Dropdown, AttributeName must not be null
            if (mapping.FieldType != null && mapping.FieldType.Equals(nameof(FieldTypes.Dropdown), StringComparison.OrdinalIgnoreCase))
            {
                if (mapping.AttributeName == null)
                {
                    yield return new ValidationResult(
                        $"Mapping[{index}]: AttributeName are required for field type.",
                        new[] { $"Mappings[{index}].Options" });
                }
            }
        }
    }
}