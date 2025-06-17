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
using RepFabric.Api.Models.Database;
using RepFabric.Api.Models.Request;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RepFabric.Api.Tests.Tests.Controller
{
    public class ExcelControllerTests
    {
        private readonly Mock<IFileStorageService> _fileStorageMock = new();
        private readonly Mock<IExcelTemplateService> _excelTemplateServiceMock = new();
        private readonly Mock<IWebHostEnvironment> _envMock = new();
        private readonly Mock<ILogger<ExcelController>> _loggerMock = new();
        private readonly IOptions<ExcelUploadSettings> _uploadSettings = Options.Create(new ExcelUploadSettings
        {
            AllowedExtensions = new[] { ".xlsm" },
            AllowedMimeTypes = new[] { "application/vnd.ms-excel.sheet.macroEnabled.12" },
            MaxFileSize = 1024 * 1024
        });

        private ExcelController CreateController()
        {
            var configMock = new Mock<IConfiguration>();
            return new ExcelController(
                _fileStorageMock.Object,
                _excelTemplateServiceMock.Object,
                configMock.Object,
                _envMock.Object,
                _loggerMock.Object,
                _uploadSettings
            );
        }


        [Fact]
        public async Task SaveMapping_ReturnsOk_WhenValid()
        {
            var controller = CreateController();
            var request = new ExcelMappingRequest
            {
                TemplateFileName = "test.xlsm",
                Mappings = new List<Mapping>
            {
                new Mapping { Cell = "A1", FieldType = "Text", AttributeName = "Id" }
            }
            };

            var result = await controller.SaveMapping(request);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetMappings_ReturnsOk()
        {
            var controller = CreateController();
            _excelTemplateServiceMock.Setup(s => s.GetTemplateMappingsAsync())
                .ReturnsAsync(new List<TemplateMapping>());

            var result = await controller.GetMappings();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteMapping_ReturnsOk_WhenDeleted()
        {
            var controller = CreateController();
            _excelTemplateServiceMock.Setup(s => s.DeleteTemplateMappingAsync(1)).ReturnsAsync(true);

            var result = await controller.DeleteMapping(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteMapping_ReturnsNotFound_WhenNotFound()
        {
            var controller = CreateController();
            _excelTemplateServiceMock.Setup(s => s.DeleteTemplateMappingAsync(1)).ReturnsAsync(false);

            var result = await controller.DeleteMapping(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateMapping_ReturnsOk_WhenUpdated()
        {
            var controller = CreateController();
            var mapping = new TemplateMapping { TemplateFileName = "test.xlsm", MappingJson = "{}" };
            _excelTemplateServiceMock.Setup(s => s.UpdateTemplateMappingAsync(1, mapping.TemplateFileName, mapping.MappingJson)).ReturnsAsync(true);

            var result = await controller.UpdateMapping(1, mapping);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateMapping_ReturnsNotFound_WhenNotFound()
        {
            var controller = CreateController();
            var mapping = new TemplateMapping { TemplateFileName = "test.xlsm", MappingJson = "{}" };
            _excelTemplateServiceMock.Setup(s => s.UpdateTemplateMappingAsync(1, mapping.TemplateFileName, mapping.MappingJson)).ReturnsAsync(false);

            var result = await controller.UpdateMapping(1, mapping);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task FillTemplate_ReturnsFile_WhenValid()
        {
            var controller = CreateController();
            _excelTemplateServiceMock.Setup(s => s.FillTemplateAsync("order1", 1)).ReturnsAsync(new byte[] { 1, 2, 3 });

            var result = await controller.FillTemplate("order1", 1);

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task GetTemplates_ReturnsOk()
        {
            var controller = CreateController();
            _excelTemplateServiceMock.Setup(s => s.ListTemplatesAsync()).ReturnsAsync(new List<string> { "a.xlsm" });

            var result = await controller.GetTemplates();

            Assert.IsType<OkObjectResult>(result);
        }
    }
}