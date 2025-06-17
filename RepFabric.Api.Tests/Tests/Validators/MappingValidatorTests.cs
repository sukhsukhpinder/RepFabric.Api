using RepFabric.Api.BL.Validators;
using RepFabric.Api.Models.Request;

namespace RepFabric.Api.Tests.Tests.Validators
{

    public class MappingValidatorTests
    {
        [Fact]
        public void ValidateAll_ReturnsValidationErrors_ForInvalidMappings()
        {
            var mappings = new List<Mapping>
        {
            new Mapping { Cell = "1A", FieldType = "InvalidType" }
        };

            var results = MappingValidator.ValidateAll(mappings);

            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Cell must be a valid Excel cell reference"));
            Assert.Contains(results, r => r.ErrorMessage.Contains("FieldType must be one of"));
        }

        [Fact]
        public void Validate_ValidMapping_ReturnsNoErrors()
        {
            var mapping = new Mapping { Cell = "B2", FieldType = "Text", AttributeName = "Test" };

            var results = MappingValidator.Validate(mapping, 0);

            Assert.Empty(results);
        }
    }
}
