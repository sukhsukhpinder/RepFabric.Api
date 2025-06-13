
using Moq;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.BL.Services;
using RepFabric.Api.Models.Request;

namespace RepFabric.Api.Tests.Tests
{

    public class ExcelTemplateServiceTests
    {
        [Fact]
        public async Task FillTemplateAsync_ReturnsFilledTemplate()
        {
            // Arrange
            var fileStorageMock = new Mock<IFileStorageService>();
            var templateBytes = File.ReadAllBytes("TestData/Template.xlsm");
            fileStorageMock.Setup(f => f.GetFileAsync(It.IsAny<string>()))
                .ReturnsAsync(new MemoryStream(templateBytes));

            var service = new ExcelTemplateService(fileStorageMock.Object);

            var request = new ExcelMappingRequest
            {
                TemplateFileName = "Template.xlsm",
                Mappings = new List<Mapping>
            {
                new Mapping { Cell = "A1", FieldType = "Text", AttributeName = "Test" }
            }
            };

            // Act
            var result = await service.FillTemplateAsync("Template.xlsm", request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }
    }
}
