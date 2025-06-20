using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RepFabric.Api.BL.Enums;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.BL.Validators;
using RepFabric.Api.Models.Common;
using RepFabric.Api.Models.DynamoDb;
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
        private readonly IDynamoDbTemplateMappingService _dynamoDbTemplateMappingService;
        private readonly IExcelTemplateService _excelTemplateService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExcelController> _logger;
        private readonly ExcelUploadSettings _excelUploadSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelController"/> class.
        /// </summary>
        public ExcelController(
            IFileStorageService fileStorage,
            IDynamoDbTemplateMappingService dynamoDbTemplateMappingService,
            IExcelTemplateService excelTemplateService,
            IConfiguration configuration,
            IWebHostEnvironment env,
            ILogger<ExcelController> logger,
            IOptions<ExcelUploadSettings> excelUploadOptions)
        {
            _fileStorage = fileStorage;
            _dynamoDbTemplateMappingService = dynamoDbTemplateMappingService;
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
        /// Saves a template mapping to the database.
        /// </summary>
        /// <param name="request">
        /// The <see cref="ExcelMappingRequest"/> object containing the template file name and mapping details.
        /// </param>
        /// <remarks>
        /// <b>Sample request:</b>
        /// <code>
        /// {
        ///   "templateFileName": "2025 Order Write-Up  02.14.25.xlsm",
        ///   "mappings": [
        ///     {
        ///       "cell": "H3",
        ///       "fieldType": "text",
        ///       "attributeName": "Id",
        ///       "formFieldName": ""
        ///     },
        ///     {
        ///       "cell": "A53",
        ///       "fieldType": "LineItem",
        ///       "attributeName": "total-price",
        ///       "formFieldName": ""
        ///     },
        ///     {
        ///       "cell": "A54",
        ///       "fieldType": "LineItem",
        ///       "attributeName": "id",
        ///       "formFieldName": ""
        ///     },
        ///     {
        ///       "cell": "C54",
        ///       "fieldType": "LineItem",
        ///       "attributeName": "unit-price",
        ///       "formFieldName": ""
        ///     },
        ///     {
        ///       "cell": "F54",
        ///       "fieldType": "LineItem",
        ///       "attributeName": "order-qty",
        ///       "formFieldName": ""
        ///     }
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        /// <returns>
        /// 200 OK if the mapping is saved successfully;<br/>
        /// 400 Bad Request if required fields are missing or invalid.
        /// </returns>
        [HttpPost("save-mapping")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveMapping([FromBody] ExcelMappingRequest request)
        {
            // Validate model using data annotations
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errors);
                _logger.LogWarning(errorMessage);
                return BadRequest(errorMessage);
            }

            // Use MappingValidator for custom mapping validation
            var mappingValidationResults = MappingValidator.ValidateAll(request.Mappings).ToList();
            if (mappingValidationResults.Any())
            {
                var errorMessages = string.Join("; ", mappingValidationResults.Select(r => r.ErrorMessage));
                _logger.LogWarning(errorMessages);
                return BadRequest(errorMessages);
            }

            // Serialize the mapping list to JSON for storage
            var mappingJson = System.Text.Json.JsonSerializer.Serialize(request.Mappings);

            await _dynamoDbTemplateMappingService.CreateAsync(request.TemplateFileName, mappingJson);
            _logger.LogInformation("Mapping saved for template {TemplateFileName}.", request.TemplateFileName);

            return Ok("Mapping saved successfully.");
        }

        /// <summary>
        /// Gets all template mappings.
        /// </summary>
        /// <returns>200 OK with a list of template mappings.</returns>
        [HttpGet("mappings")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<TemplateMapping>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMappings()
        {
            var mappings = await _dynamoDbTemplateMappingService.GetAllAsync();
            return Ok(mappings);
        }

        /// <summary>
        /// Deletes a template mapping by its ID.
        /// </summary>
        /// <param name="id">The ID of the template mapping to delete.</param>
        /// <returns>
        /// 200 OK if the mapping is deleted successfully;<br/>
        /// 404 Not Found if the mapping does not exist.
        /// </returns>
        [HttpDelete("mapping/{id:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMapping([FromRoute] int id)
        {
            await _dynamoDbTemplateMappingService.DeleteAsync(id);
            _logger.LogInformation("Mapping with id {Id} deleted.", id);
            return Ok("Mapping deleted successfully.");
        }

        /// <summary>
        /// Updates an existing template mapping.
        /// </summary>
        /// <param name="id">The ID of the template mapping to update.</param>
        /// <param name="request">
        /// The <see cref="TemplateMapping"/> object containing the updated template file name and mapping JSON.
        /// </param>
        /// <returns>
        /// 200 OK if the mapping is updated successfully;<br/>
        /// 400 Bad Request if required fields are missing;<br/>
        /// 404 Not Found if the mapping does not exist.
        /// </returns>
        [HttpPut("mapping/{id:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMapping([FromRoute] int id, [FromBody] TemplateMapping request)
        {
            if (string.IsNullOrWhiteSpace(request.TemplateFileName) || string.IsNullOrWhiteSpace(request.MappingJson))
            {
                _logger.LogWarning("TemplateFileName and MappingJson are required for update.");
                return BadRequest("TemplateFileName and MappingJson are required.");
            }

            await _dynamoDbTemplateMappingService.UpdateAsync(id, request);

            _logger.LogInformation("Mapping with id {Id} updated.", id);
            return Ok("Mapping updated successfully.");
        }


        /// <summary>
        /// Fills an uploaded Excel template with provided mapping data and returns the filled file.
        /// </summary>
        /// <returns>
        /// 200 OK with the filled Excel file;
        /// 400 Bad Request if validation fails;
        /// 404 Not Found if the template does not exist.
        /// </returns>
        [HttpGet("fill-template/{orderId}/{mappingId}")]
        [Produces("application/vnd.ms-excel.sheet.macroEnabled.12", "application/json")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FillTemplate([FromQuery] string orderId, [FromQuery] int mappingId)
        {
            if (string.IsNullOrWhiteSpace(orderId) || mappingId <= 0)
            {
                _logger.LogWarning("Invalid orderId or mappingId.");
                return BadRequest("orderId and mappingId are required.");
            }

            byte[] result;
            try
            {
                result = await _excelTemplateService.FillTemplateAsync(orderId, mappingId);
                _logger.LogInformation($"Excel template filled for orderId {orderId} and mappingId {mappingId}.");
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning($"Template mapping with id {mappingId} not found.");
                return NotFound("Template mapping not found.");
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
