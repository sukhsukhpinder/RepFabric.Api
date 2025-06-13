
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Controllers;
using RepFabric.Api.Models.Common;
using RepFabric.Api.Models.Request;

namespace RepFabric.Api.Tests.Tests
{

    public class ExcelControllerTests
    {
        [Fact]
        public async Task UploadTemplate_InvalidModel_ReturnsBadRequest()
        {
            var controller = GetController();
            controller.ModelState.AddModelError("File", "Required");

            var result = await controller.UploadTemplate(new UploadTemplateRequest());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UploadTemplate_InvalidFile_ReturnsBadRequest()
        {
            var controller = GetController();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);
            var request = new UploadTemplateRequest { File = fileMock.Object };

            var result = await controller.UploadTemplate(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UploadTemplate_ValidFile_ReturnsOk()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("test.xlsm");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[100]));

            var storageMock = new Mock<IFileStorageService>();
            storageMock.Setup(s => s.SaveFileAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);

            var controller = GetController(storageMock.Object);
            var request = new UploadTemplateRequest { File = fileMock.Object };

            var result = await controller.UploadTemplate(request);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task FillTemplate_InvalidModel_ReturnsBadRequest()
        {
            var controller = GetController();
            controller.ModelState.AddModelError("TemplateFileName", "Required");

            var result = await controller.FillTemplate(new ExcelMappingRequest());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task FillTemplate_ValidationFails_ReturnsBadRequest()
        {
            var controller = GetController();
            var request = new ExcelMappingRequest
            {
                TemplateFileName = "file.xlsm",
                Mappings = new List<Mapping> { new Mapping { Cell = "1A", FieldType = "Invalid" } }
            };

            var result = await controller.FillTemplate(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task FillTemplate_TemplateNotFound_ReturnsNotFound()
        {
            var excelServiceMock = new Mock<IExcelTemplateService>();
            excelServiceMock.Setup(s => s.FillTemplateAsync(It.IsAny<string>(), It.IsAny<ExcelMappingRequest>()))
                .ThrowsAsync(new FileNotFoundException());

            var controller = GetController(excelTemplateService: excelServiceMock.Object);

            var request = new ExcelMappingRequest
            {
                TemplateFileName = "file.xlsm",
                Mappings = new List<Mapping> { new Mapping { Cell = "A1", FieldType = "Text", AttributeName = "Test" } }
            };

            var result = await controller.FillTemplate(request);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task FillTemplate_ValidRequest_ReturnsFile()
        {
            var excelServiceMock = new Mock<IExcelTemplateService>();
            excelServiceMock.Setup(s => s.FillTemplateAsync(It.IsAny<string>(), It.IsAny<ExcelMappingRequest>()))
                .ReturnsAsync(new byte[] { 1, 2, 3 });

            var controller = GetController(excelTemplateService: excelServiceMock.Object);

            var request = new ExcelMappingRequest
            {
                TemplateFileName = "file.xlsm",
                Mappings = new List<Mapping> { new Mapping { Cell = "A1", FieldType = "Text", AttributeName = "Test" } }
            };

            var result = await controller.FillTemplate(request);

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task GetTemplates_ReturnsTemplatesList()
        {
            // Arrange
            var templates = new List<string> { "template1.xlsm", "template2.xlsm" };
            var excelServiceMock = new Mock<IExcelTemplateService>();
            excelServiceMock.Setup(s => s.ListTemplatesAsync()).ReturnsAsync(templates);

            var loggerMock = new Mock<ILogger<ExcelController>>();
            var controller = GetController(excelTemplateService: excelServiceMock.Object, logger: loggerMock.Object);

            // Act
            var result = await controller.GetTemplates();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTemplates = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
            Assert.Equal(templates, returnedTemplates);

            // Logger should be called with the count
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Retrieved 2 Excel templates.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetTemplates_ReturnsEmptyList_WhenNoTemplates()
        {
            // Arrange
            var excelServiceMock = new Mock<IExcelTemplateService>();
            excelServiceMock.Setup(s => s.ListTemplatesAsync()).ReturnsAsync(new List<string>());

            var loggerMock = new Mock<ILogger<ExcelController>>();
            var controller = GetController(excelTemplateService: excelServiceMock.Object, logger: loggerMock.Object);

            // Act
            var result = await controller.GetTemplates();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTemplates = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
            Assert.Empty(returnedTemplates);

            // Logger should be called with count 0
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Retrieved 0 Excel templates.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        private ExcelController GetController(
            IFileStorageService fileStorage = null,
            IExcelTemplateService excelTemplateService = null,
            ILogger<ExcelController> logger = null)
        {
            fileStorage ??= Mock.Of<IFileStorageService>();
            excelTemplateService ??= Mock.Of<IExcelTemplateService>();
            logger ??= Mock.Of<ILogger<ExcelController>>();

            // Use in-memory configuration
            var inMemorySettings = new Dictionary<string, string>
            {
                { "ExcelUpload:AllowedExtensions:0", ".xlsm" }
            };
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var env = Mock.Of<IWebHostEnvironment>();
            var options = Options.Create(new ExcelUploadSettings
            {
                AllowedExtensions = new[] { ".xlsm" },
                MaxFileSize = 1024 * 1024
            });

            // ... rest of your setup ...
            return new ExcelController(fileStorage, excelTemplateService, config, env, logger, options);
        }
    }
}
