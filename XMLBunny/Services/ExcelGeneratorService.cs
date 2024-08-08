using ClosedXML.Excel;
using XMLBunny.Models;

namespace XMLBunny.Services;

public class ExcelGeneratorService
{
    public void SaveEmptyTags(IXLWorksheet sheet, List<Tag> tags, int row, int column)
    {
        var emptyTags = tags.Where(x => x.Values.Count <= 0).ToList();
        sheet.Cell(row, column).Value = "Tag";
        sheet.Cell(row, column + 1).Value = "Count";
        row += 1;
        
        for (int i = 0; i < emptyTags.Count(); i++)
        {
            sheet.Cell(row, column).Value = emptyTags[i].Name;
            sheet.Cell(row, column + 1).Value = emptyTags[i].Count;
            row += 1;
            if (i == emptyTags.Count() - 1)
            {
                row = 1;
                column += 2;
            }
        }
    }
    
    public void SaveValueTags(IXLWorksheet sheet, List<Tag> tags, int row, int column)
    {
        var valueTags = tags.Where(x => x.Values.Count > 0).ToList();
        for (int i = 0; i < valueTags.Count(); i++)
        {
            sheet.Cell(row, column).Value = valueTags[i].Name;
            sheet.Cell(row, column + 1).Value = "Count";
            row += 1;
            for (int j = 0; j < valueTags[i].Values.Count(); j++)
            {
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
    }
    
    public void SaveRanges(IXLWorksheet sheet, List<NumberRange> ranges, int row, int column)
    {
        sheet.Cell(row, column).Value = "Tag";
        sheet.Cell(row, column + 1).Value = "Range";
        row += 1;
        for (int i = 0; i < ranges.Count(); i++)
        {
            if (ranges[i].Numbers.Count > 1 && ranges[i].Numbers.Min() != ranges[i].Numbers.Max())
            { 
                sheet.Cell(row, column).Value = ranges[i].Tag.Name;
                sheet.Cell(row, column + 1).Value = $"{ranges[i].Numbers.Min()} – {ranges[i].Numbers.Max()}";
                row += 1;
            }
        }
    }
}
