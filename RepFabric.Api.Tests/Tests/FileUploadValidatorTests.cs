
using Microsoft.AspNetCore.Http;
using Moq;
using RepFabric.Api.BL.Validators;

namespace RepFabric.Api.Tests.Tests
{

    public class FileUploadValidatorTests
    {
        [Fact]
        public void Validate_NullFile_ReturnsError()
        {
            var validator = new FileUploadValidator(new[] { ".xlsm" }, new[] { "application/vnd.ms-excel.sheet.macroEnabled.12" }, 1024 * 1024);
            var (isValid, error) = validator.Validate(null);

            Assert.False(isValid);
            Assert.Equal("No file uploaded.", error);
        }

        [Fact]
        public void Validate_FileTooLarge_ReturnsError()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(2 * 1024 * 1024);
            fileMock.Setup(f => f.FileName).Returns("test.xlsm");

            var validator = new FileUploadValidator(new[] { ".xlsm" }, new[] { "application/vnd.ms-excel.sheet.macroEnabled.12" }, 1024 * 1024);
            var (isValid, error) = validator.Validate(fileMock.Object);

            Assert.False(isValid);
            Assert.Contains("File size exceeds", error);
        }

        [Fact]
        public void Validate_InvalidExtension_ReturnsError()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("test.txt");

            var validator = new FileUploadValidator(new[] { ".xlsm" }, new[] { "application/vnd.ms-excel.sheet.macroEnabled.12" }, 1024 * 1024);
            var (isValid, error) = validator.Validate(fileMock.Object);

            Assert.False(isValid);
            Assert.Contains(".xlsm", error);
        }

        [Fact]
        public void Validate_ValidFile_ReturnsTrue()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("test.xlsm");

            var validator = new FileUploadValidator(new[] { ".xlsm" }, new[] { "application/vnd.ms-excel.sheet.macroEnabled.12" }, 1024 * 1024);
            var (isValid, error) = validator.Validate(fileMock.Object);

            Assert.True(isValid);
            Assert.Equal(string.Empty, error);
        }
    }
}
