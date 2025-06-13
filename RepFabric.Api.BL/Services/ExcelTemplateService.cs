using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using RepFabric.Api.BL.Enums;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Models.Request;
using System.Text.RegularExpressions;

namespace RepFabric.Api.BL.Services
{
    public class ExcelTemplateService : IExcelTemplateService
    {
        private readonly IFileStorageService _fileStorage;
        private readonly string _templateFolder;

        public ExcelTemplateService(IFileStorageService fileStorage)
        {
            _fileStorage = fileStorage;
            // Assume local folder is "Templates" for local storage
            _templateFolder = "Templates";
        }

        public async Task<byte[]> FillTemplateAsync(string templateFileName, ExcelMappingRequest request)
        {
            Stream templateStream = await _fileStorage.GetFileAsync(templateFileName);

            using var ms = new MemoryStream();
            await templateStream.CopyToAsync(ms);
            ms.Position = 0;

            using (var document = SpreadsheetDocument.Open(ms, true))
            {
                var workbookPart = document.WorkbookPart!;
                var worksheetPart = workbookPart.WorksheetParts.First();
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                foreach (var mapping in request.Mappings)
                {
                    var cellRef = mapping.Cell.ToUpperInvariant();
                    var cell = GetOrCreateCell(sheetData, cellRef);

                    // Always update the cell value
                    cell.CellValue = new CellValue(mapping.AttributeName ?? string.Empty);
                    cell.DataType = CellValues.String;

                    // If dropdown, update the existing data validation options if present
                    if (mapping.FieldType.Equals(nameof(FieldTypes.Dropdown), StringComparison.OrdinalIgnoreCase) && mapping.AttributeName is not null)
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

        // Utility to get or create a cell in OpenXML
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