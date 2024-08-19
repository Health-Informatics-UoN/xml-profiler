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
GenerateStatistics();

void GenerateStatistics()
{ 
    try
    {
        var (filePath, fileName, threshold) = GetUserInputs();
        var xmlDocument = LoadXmlDocument(filePath, fileName);
        if (xmlDocument.DocumentElement != null)
        { 
            xmlService.ParseXml(xmlDocument.DocumentElement, tags, values, ranges);
            SaveDataToExcel(fileName, threshold);
        }
        else
        {
            Console.WriteLine("Something went wrong loading the XML file");
            GenerateStatistics();
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error: {e.Message}");
        GenerateStatistics();
    }
}

static (string filePath, string fileName, int threshold) GetUserInputs()
{
    string GetInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }
    
    var filePath = GetInput("Enter Path To XML File: ");
    var fileName = GetInput("Enter XML File Name: ");
    var threshold = int.Parse(GetInput("Enter The Minimum Count Threshold: "));

    return (filePath, fileName, threshold);
}

static XmlDocument LoadXmlDocument(string filePath, string fileName)
{
    var root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    var path = Path.Combine(root, filePath, fileName);
    var doc = new XmlDocument();
    doc.Load(path);
        
    if (doc.DocumentElement == null)
        throw new NullReferenceException("Something went wrong loading the XML file");

    return doc;
}

void SaveDataToExcel(string fileName, int threshold)
{
    var row = 1;
    var column = 1;
    var workbook = new XLWorkbook();
    var sheetName = fileName.Replace(".xml", string.Empty);
    var sheet = workbook.Worksheets.Add(sheetName);

    excelService.SaveToExcel(sheet, tags, ranges, threshold, row, column);

    var outputPath = GetOutputPath(fileName);
    workbook.SaveAs(outputPath);
    Console.WriteLine($"Data has been saved to {outputPath}");

    Restart();
}

static string GetOutputPath(string fileName)
{
    Console.Write("Enter The Path To Save The Output: ");
    var path = Console.ReadLine()?.Trim() ?? string.Empty;
    var root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    return Path.Combine(root, path, fileName.Replace(".xml", ".xlsx"));
}

void Restart()
{
    Console.Write("Do you want to parse another file? (yes/no): ");
    var response = Console.ReadLine()?.Trim().ToLower();

    switch (response)
    {
        case "yes":
            ResetState();
            GenerateStatistics();
            break;
        case "no":
            Environment.Exit(0);
            break;
        default:
            Console.WriteLine("Please provide valid input (yes/no)");
            Restart();
            break;
    }
}

void ResetState()
{
    Console.Clear();
    tags.Clear();
    values.Clear();
    ranges.Clear();
}
