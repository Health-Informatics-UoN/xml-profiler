using System.Xml;
using ClosedXML.Excel;
using XMLBunny.Models;
using XMLBunny.Services;

var tags = new List<Tag>();
var values = new List<Value>();
var ranges = new List<NumberRange>();
var xmlService = new XmlParserService();
var excelService = new ExcelGeneratorService();

Console.Clear();

void GenerateStatistics()
{ 
    Console.Write("Enter Path To XML File: ");
    var filePath = Console.ReadLine() ?? string.Empty;
    Console.Write("Enter XML File Name: ");
    var fileName = Console.ReadLine() ?? string.Empty;
    Console.Write("Enter The Minimum Count Threshold: ");
    var threshold = Int32.Parse(Console.ReadLine() ?? string.Empty);
    
    try
    {
        var doc = new XmlDocument();
        doc.Load($"{filePath}/{fileName}");
        if (doc.DocumentElement == null) throw new NullReferenceException("Something went wrong loading the XML file");
        xmlService.ParseXml(doc.DocumentElement, tags, values, ranges);
        SaveDataToExcel(fileName, threshold);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error: {e.Message}");
        GenerateStatistics();
    }
}

void SaveDataToExcel(string fileName, int threshold)
{
    var row = 1;
    var column = 1;
    var workbook = new XLWorkbook();
    var sheet = workbook.Worksheets.Add(fileName.Replace(".xml",""));
    
    excelService.SaveToExcel(sheet, tags, ranges, threshold, row, column);
    
    Console.Write("Enter The Path To Save The Output: ");
    var path = Console.ReadLine() ?? string.Empty;
    var outputFilePath = Environment.CurrentDirectory + $"/{path}" + $"/{fileName.Replace(".xml", ".xlsx")}";
    workbook.SaveAs(outputFilePath);
    Console.WriteLine($"Data has been saved to {outputFilePath}");

    Restart();
}

void Restart()
{
    Console.Write("Do you want to parse another file?: ");
    var response = Console.ReadLine() ?? "";
    
    if (response.ToLower().Trim() == "yes")
    {
        Console.Clear();
        tags.Clear();
        values.Clear();
        ranges.Clear();
        GenerateStatistics();
    } 
    else if (response.ToLower().Trim() == "no")
    {
        Environment.Exit(0);
    }
    else
    {
        Console.WriteLine("Please provide valid input (YES/NO)");
        Restart();
    }
}

GenerateStatistics();
