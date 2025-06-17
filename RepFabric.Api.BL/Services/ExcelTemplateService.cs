using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RepFabric.Api.BL.Constants;
using RepFabric.Api.BL.Database;
using RepFabric.Api.BL.Enums;
using RepFabric.Api.BL.Helper;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Models.Common;
using RepFabric.Api.Models.Database;
using RepFabric.Api.Models.Request;
using RepFabric.Api.Models.Response;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace RepFabric.Api.BL.Services
{
    /// <summary>
    /// Service for managing Excel templates and their mappings, including
    /// filling templates with order data, listing available templates, and CRUD operations
    /// for template mappings. Implements <see cref="IExcelTemplateService"/>.
    /// </summary>
    public class ExcelTemplateService : IExcelTemplateService
    {
        private readonly IFileStorageService _fileStorage;
        private readonly string _templateFolder;
        private readonly IYoxelSyncService _yoxelSyncService;
        private readonly YoxelSettings _yoxelSettings;
        private readonly RepFabricContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelTemplateService"/> class.
        /// </summary>
        /// <param name="fileStorage">Service for file storage operations.</param>
        /// <param name="yoxelSyncService">Service for retrieving order data.</param>
        /// <param name="yoxelSettings">Yoxel integration settings.</param>
        /// <param name="dbContext">Database context for template mappings.</param>
        public ExcelTemplateService(
            IFileStorageService fileStorage,
            IYoxelSyncService yoxelSyncService,
            IOptions<YoxelSettings> yoxelSettings,
            RepFabricContext dbContext)
        {
            _fileStorage = fileStorage;
            // Assume local folder is "Templates" for local storage
            _templateFolder = "Templates";
            _yoxelSyncService = yoxelSyncService;
            _yoxelSettings = yoxelSettings.Value;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves order details asynchronously from the Yoxel service.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>The purchase order response.</returns>
        private async Task<PurchaseOrderResponse> GetOrderDetailsAysnc(string orderId)
        {
            return await _yoxelSyncService.GetAsync<PurchaseOrderResponse>(
                            _yoxelSettings.BaseAddress,
                            YoxelConstants.PurchaseOrder,
                            new Dictionary<string, string> { { "orderId", orderId } },
                            _yoxelSettings.AuthToken
                        );
        }

        /// <inheritdoc/>
        public async Task SaveTemplateMappingAsync(string templateFileName, string mappingJson)
        {
            var entity = new TemplateMapping
            {
                TemplateFileName = templateFileName,
                MappingJson = mappingJson
            };

            _dbContext.TemplateMappings.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteTemplateMappingAsync(int id)
        {
            var entity = await _dbContext.TemplateMappings.FindAsync(id);
            if (entity == null)
                return false;

            _dbContext.TemplateMappings.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TemplateMapping>> GetTemplateMappingsAsync()
        {
            return await _dbContext.TemplateMappings.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateTemplateMappingAsync(int id, string templateFileName, string mappingJson)
        {
            var entity = await _dbContext.TemplateMappings.FindAsync(id);
            if (entity == null)
                return false;

            entity.TemplateFileName = templateFileName;
            entity.MappingJson = mappingJson;
            _dbContext.TemplateMappings.Update(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <inheritdoc/>
        public async Task<byte[]> FillTemplateAsync(string orderId, int mappingId)
        {
            // Retrieve the template mapping from the database
            var template = await _dbContext.TemplateMappings.Where(m => m.Id == mappingId).FirstOrDefaultAsync();

            // Get the template file as a stream
            Stream templateStream = await _fileStorage.GetFileAsync(template.TemplateFileName);

            using var ms = new MemoryStream();
            await templateStream.CopyToAsync(ms);
            ms.Position = 0;

            // Open and fill the Excel template using OpenXML
            using (var document = SpreadsheetDocument.Open(ms, true))
            {
                var workbookPart = document.WorkbookPart!;
                var worksheetPart = workbookPart.WorksheetParts.First();
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                var orderDetails = await GetOrderDetailsAysnc(orderId);
                var templateMapping = JsonSerializer.Deserialize<List<Mapping>>(template.MappingJson) ?? new List<Mapping>();
                foreach (var mapping in templateMapping)
                {
                    var cellRef = mapping.Cell.ToUpperInvariant();
                    var cell = GetOrCreateCell(sheetData, cellRef);

                    if (mapping.FieldType.Equals(nameof(FieldTypes.Text), StringComparison.OrdinalIgnoreCase) && mapping.AttributeName is not null)
                    {
                        // Update the cell value with text data
                        var value = PurchaseOrderResponseHelper.GetByJsonPropertyName<string>(orderDetails, mapping.AttributeName);
                        cell.CellValue = new CellValue(value ?? string.Empty);
                        cell.DataType = CellValues.String;
                    }
                    else if (mapping.FieldType.Equals(nameof(FieldTypes.LineItem), StringComparison.OrdinalIgnoreCase) && mapping.AttributeName is not null)
                    {
                        // Fill line item values in consecutive cells
                        var values = orderDetails.LineItems.GetValuesByJsonPropertyName<int>(mapping.AttributeName);
                        string startCellRef = mapping.Cell.ToUpperInvariant();

                        for (int i = 0; i < values.Count; i++)
                        {
                            var currentCellRef = IncrementCellReference(startCellRef, i);
                            cell = GetOrCreateCell(sheetData, currentCellRef);
                            cell.CellValue = new CellValue(values[i]);
                            cell.DataType = CellValues.Number;
                        }
                    }
                    // Update dropdown data validation options if present
                    else if (mapping.FieldType.Equals(nameof(FieldTypes.Dropdown), StringComparison.OrdinalIgnoreCase) && mapping.AttributeName is not null)
                    {
                        var validations = worksheetPart.Worksheet.Elements<DataValidations>().FirstOrDefault();
                        if (validations != null)
                        {
                            foreach (var validation in validations.Elements<DataValidation>())
                            {
                                if (validation.SequenceOfReferences != null &&
                                    validation.SequenceOfReferences.InnerText.Split(' ').Any(r => r.Equals(cellRef, StringComparison.OrdinalIgnoreCase)))
                                {
                                    validation.Formula1 = new Formula1("\"" + string.Join(",", mapping.AttributeName) + "\"");
                                }
                            }
                        }
                    }
                }
                worksheetPart.Worksheet.Save();
            }

            ms.Position = 0;
            return ms.ToArray();
        }

        /// <summary>
        /// Increments the row number in a cell reference (e.g., A1 to A2).
        /// </summary>
        /// <param name="cellRef">The starting cell reference.</param>
        /// <param name="increment">The number of rows to increment.</param>
        /// <returns>The incremented cell reference.</returns>
        private static string IncrementCellReference(string cellRef, int increment)
        {
            var match = Regex.Match(cellRef, @"^([A-Z]+)(\d+)$");
            if (!match.Success)
                throw new ArgumentException("Invalid cell reference.");

            var col = match.Groups[1].Value;
            var row = int.Parse(match.Groups[2].Value);
            return $"{col}{row + increment}";
        }

        /// <summary>
        /// Gets or creates a cell in the specified sheet data by cell reference.
        /// </summary>
        /// <param name="sheetData">The sheet data.</param>
        /// <param name="cellReference">The cell reference (e.g., "A1").</param>
        /// <returns>The cell object.</returns>
        private static Cell GetOrCreateCell(SheetData sheetData, string cellReference)
        {
            var match = Regex.Match(cellReference, @"([A-Z]+)(\d+)");
            if (!match.Success)
                throw new ArgumentException("Invalid cell reference.");

            var rowIndex = uint.Parse(match.Groups[2].Value);
            var row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            if (row == null)
            {
                row = new Row { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            var cell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference == cellReference);
            if (cell == null)
            {
                cell = new Cell { CellReference = cellReference };
                row.Append(cell);
            }
            return cell;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> ListTemplatesAsync()
        {
            // For local storage, list files in the folder
            if (Directory.Exists(_templateFolder))
            {
                var files = Directory.GetFiles(_templateFolder)
                                     .Select(Path.GetFileName);
                return await Task.FromResult(files);
            }
            return Enumerable.Empty<string>();
        }
    }
}