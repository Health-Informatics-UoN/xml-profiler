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
    var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    Console.Write("Enter XML File Name: ");
    var fileName = Console.ReadLine() ?? string.Empty;
    var filePath = path + $"/{fileName}";

    try
    {
        var doc = new XmlDocument();
        doc.Load(filePath);
        if (doc.DocumentElement == null) throw new NullReferenceException("Something went wrong loading the XML file");
        xmlService.ParseXml(doc.DocumentElement, tags, values, ranges);
        SaveDataToExcel(fileName);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error: {e.Message}");
        GenerateStatistics();
    }
}

void SaveDataToExcel(string fileName)
{
    var workbook = new XLWorkbook();
    var sheet = workbook.Worksheets.Add(fileName.Replace(".xml",""));
    var row = 1;
    var column = 1;
    
    excelService.SaveToExcel(sheet, tags, ranges, row, column);
    
    var outputFilePath = Environment.CurrentDirectory + $"/{fileName.Replace(".xml", ".xlsx")}";
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
