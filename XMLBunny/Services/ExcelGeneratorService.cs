using ClosedXML.Excel;
using XMLBunny.Models;

namespace XMLBunny.Services;

public class ExcelGeneratorService
{
    /// <summary>
    /// Generates an Excel file with the parsed XML data.
    /// </summary>
    /// <param name="fileName">The name of the Excel file to be created.</param>
    /// <param name="tags">A collection of tags to be included in the Excel file.</param>
    /// <param name="ranges">A collection of number ranges to be included in the Excel file.</param>
    /// <param name="threshold">The minimum count threshold for displaying tags and values.</param>
    /// <param name="filePath">The path where the Excel file will be saved.</param>
    public void GenerateExcel(string fileName, List<Tag> tags, List<NumberRange> ranges, int threshold, string filePath)
    {
        var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("XML Data");
        
        SaveToExcel(sheet, tags, ranges, threshold, 1, 1);
        
        var root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var path = Path.Combine(root, filePath);
        workbook.SaveAs(Path.Combine(path, $"{fileName}.xlsx"));
    }

    /// <summary>
    /// Saves the parsed XML data to an Excel worksheet.
    /// </summary>
    /// <param name="sheet">The Excel worksheet to save data to.</param>
    /// <param name="tags">A collection of tags to be included in the Excel file.</param>
    /// <param name="ranges">A collection of number ranges to be included in the Excel file.</param>
    /// <param name="threshold">The minimum count threshold for displaying tags and values.</param>
    /// <param name="row">The starting row for writing data.</param>
    /// <param name="column">The starting column for writing data.</param>
    public void SaveToExcel(IXLWorksheet sheet, List<Tag> tags, List<NumberRange> ranges, int threshold, int row, int column)
    {
        // Add empty tags to excel
        var emptyTags = tags.Where(x => x.Values.Count <= 0).ToList();
        sheet.Cell(row, column).Value = "Tag";
        sheet.Cell(row, column + 1).Value = "Count";
        row += 1;
        for (int i = 0; i < emptyTags.Count(); i++)
        {
            if (emptyTags.Count() < threshold)
            {
                sheet.Cell(row, column).Value = "List Truncated...";
                row = 1;
                column += 2;
                break;
            }
            
            sheet.Cell(row, column).Value = emptyTags[i].Name;
            sheet.Cell(row, column + 1).Value = emptyTags[i].Count;
            row += 1;
            if (i == emptyTags.Count() - 1)
            {
                row = 1;
                column += 2;
            }
        }
        
        // Add value tags to excel
        SaveValueTags(sheet, tags, ranges, threshold, row, column);
    }
    
    /// <summary>
    /// Saves value tags and their associated ranges to the Excel worksheet.
    /// </summary>
    /// <param name="sheet">The Excel worksheet to save data to.</param>
    /// <param name="tags">A collection of tags to be included in the Excel file.</param>
    /// <param name="ranges">A collection of number ranges to be included in the Excel file.</param>
    /// <param name="threshold">The minimum count threshold for displaying tags and values.</param>
    /// <param name="row">The starting row for writing data.</param>
    /// <param name="column">The starting column for writing data.</param>
    private void SaveValueTags(IXLWorksheet sheet, List<Tag> tags, List<NumberRange> ranges, int threshold, int row, int column)
    {
        var valueTags = tags.Where(x => x.Values.Count > 0).ToList();
        for (int i = 0; i < valueTags.Count(); i++)
        {
            sheet.Cell(row, column).Value = valueTags[i].Name;
            sheet.Cell(row, column + 1).Value = "Count";
            row += 1;
            for (int j = 0; j < valueTags[i].Values.Count(); j++)
            {
                if (valueTags[i].Values.Count() < threshold)
                {
                    sheet.Cell(row, column).Value = "List Truncated...";
                    row = 1;
                    column += 2;
                    break;
                }
                    
                sheet.Cell(row, column).Value = valueTags[i].Values[j].Name;
                sheet.Cell(row, column + 1).Value = valueTags[i].Values[j].Count;
                row += 1;
                if (j == valueTags[i].Values.Count() - 1)
                {
                    row = 1;
                    column += 2;
                }
            }
        }
        
        // Add ranges to excel
        SaveRanges(sheet, ranges, threshold, row, column);
    }

    /// <summary>
    /// Saves number ranges and their associated tags to the Excel worksheet.
    /// </summary>
    /// <param name="sheet">The Excel worksheet to save data to.</param>
    /// <param name="ranges">A collection of number ranges to be included in the Excel file.</param>
    /// <param name="threshold">The minimum count threshold for displaying tags and values.</param>
    /// <param name="row">The starting row for writing data.</param>
    /// <param name="column">The starting column for writing data.</param>
    private void SaveRanges(IXLWorksheet sheet, List<NumberRange> ranges, int threshold, int row, int column)
    {
        var exists = ranges.Find(x => x.Tag.Count > threshold);
        if (exists != null)
        {
            sheet.Cell(row, column).Value = "Tag";
            sheet.Cell(row, column + 1).Value = "Range";
            row += 1;
            for (int i = 0; i < ranges.Count(); i++)
            {
                if (ranges[i].Tag.Count < threshold) continue;
            
                if (ranges[i].Numbers.Count > 1 && ranges[i].Numbers.Min() != ranges[i].Numbers.Max())
                { 
                    sheet.Cell(row, column).Value = ranges[i].Tag.Name;
                    sheet.Cell(row, column + 1).Value = $"{ranges[i].Numbers.Min()} – {ranges[i].Numbers.Max()}";
                    row += 1;
                }
            }
        }
    }
}
