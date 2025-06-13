using RepFabric.Api.BL.Enums;
using RepFabric.Api.Models.Request;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RepFabric.Api.BL.Validators
{
    public static class MappingValidator
    {
        private static readonly string[] AllowedFieldTypes = { nameof(FieldTypes.Text), nameof(FieldTypes.Dropdown) }; // Extend as needed
        public static IEnumerable<ValidationResult> ValidateAll(IEnumerable<Mapping> mappings)
        {
            return mappings
                .SelectMany((mapping, i) => Validate(mapping, i))
                .ToList();
        }
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

            // If FieldType is Dropdown, Options must not be null/empty and Value must be in Options
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