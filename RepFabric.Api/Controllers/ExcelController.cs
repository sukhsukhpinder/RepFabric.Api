using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RepFabric.Api.BL.Enums;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.BL.Validators;
using RepFabric.Api.Models.Common;
using RepFabric.Api.Models.Request;
using RepFabric.Api.Models.Response;

namespace RepFabric.Api.Controllers
{
    /// <summary>
    /// API controller for managing Excel template upload, fill, and listing operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelController : ControllerBase
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IExcelTemplateService _excelTemplateService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExcelController> _logger;
        private readonly ExcelUploadSettings _excelUploadSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelController"/> class.
        /// </summary>
        public ExcelController(
            IFileStorageService fileStorage,
            IExcelTemplateService excelTemplateService,
            IConfiguration configuration,
            IWebHostEnvironment env,
            ILogger<ExcelController> logger,
            IOptions<ExcelUploadSettings> excelUploadOptions)
        {
            _fileStorage = fileStorage;
            _excelTemplateService = excelTemplateService;
            _env = env;
            _logger = logger;
            _excelUploadSettings = excelUploadOptions.Value;
        }

        /// <summary>
        /// Uploads an Excel template file to the configured storage.
        /// </summary>
        /// <param name="request">The upload request containing the Excel file.</param>
        /// <returns>
        /// 200 OK with upload details if successful;
        /// 400 Bad Request if validation fails.
        /// </returns>
        [HttpPost("upload-template")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UploadTemplateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadTemplate([FromForm] UploadTemplateRequest request)
        {
            var file = request.File;

            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Invalid request.";
                _logger.LogWarning(error);
                return BadRequest(error);
            }

            var validator = new FileUploadValidator(
                _excelUploadSettings.AllowedExtensions,
                _excelUploadSettings.AllowedMimeTypes,
                _excelUploadSettings.MaxFileSize);

            var (isValid, errorMessage) = validator.Validate(file);

            if (!isValid)
            {
                _logger.LogWarning(errorMessage);
                return BadRequest(errorMessage);
            }

            using var stream = file.OpenReadStream();
            await _fileStorage.SaveFileAsync(file.FileName, stream);
            _logger.LogInformation($"File {file.FileName} uploaded successfully.");

            // Determine storage type
            string storageType = _fileStorage.GetType().Name.ToLower().Contains(nameof(StorageTypes.S3)) ? nameof(StorageTypes.S3) : nameof(StorageTypes.Local);

            var response = new UploadTemplateResponse
            {
                Message = "File uploaded successfully.",
                FileName = file.FileName,
                Storage = storageType
            };

            return Ok(response);
        }

        /// <summary>
        /// Fills an uploaded Excel template with provided mapping data and returns the filled file.
        /// </summary>
        /// <param name="request">The mapping request containing the template file name and cell mappings.</param>
        /// <returns>
        /// 200 OK with the filled Excel file;
        /// 400 Bad Request if validation fails;
        /// 404 Not Found if the template does not exist.
        /// </returns>
        [HttpPost("fill-template")]
        [Produces("application/vnd.ms-excel.sheet.macroEnabled.12", "application/json")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FillTemplate([FromBody] ExcelMappingRequest request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Invalid request.";
                _logger.LogWarning(error);
                return BadRequest(error);
            }

            var mappingValidationResults = MappingValidator.ValidateAll(request.Mappings).ToList();
            if (mappingValidationResults.Any())
            {
                var errorMessages = string.Join("; ", mappingValidationResults.Select(r => r.ErrorMessage));
                _logger.LogWarning(errorMessages);
                return BadRequest(errorMessages);
            }

            byte[] result;
            try
            {
                result = await _excelTemplateService.FillTemplateAsync(request.TemplateFileName, request);
                _logger.LogInformation($"Excel {request.TemplateFileName} generated successfully.", request.TemplateFileName);
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning($"Template file {request.TemplateFileName} not found.");
                return NotFound("Template not found.");
            }

            return File(result,
                        "application/vnd.ms-excel.sheet.macroEnabled.12",
                        "filled-template.xlsm");
        }

        /// <summary>
        /// Gets the list of available Excel template file names.
        /// </summary>
        /// <returns>
        /// 200 OK with a list of template file names.
        /// </returns>
        [HttpGet("templates")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTemplates()
        {
            var templates = await _excelTemplateService.ListTemplatesAsync();
            _logger.LogInformation("Retrieved {Count} Excel templates.", templates?.Count() ?? 0);
            return Ok(templates);
        }
    }
}
