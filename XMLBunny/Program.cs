using System.Text.RegularExpressions;
using System.Xml;
using ConsoleTables;
using XMLBunny;
using Range = XMLBunny.Range;

var tags = new List<Tag>();
var values = new List<Value>();
var ranges = new List<Range>();

Console.Clear();

void GenerateStatistics()
{ 
    // Get the file path from Desktop
    var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    Console.Write("Enter XML File Name: ");
    var fileName = Console.ReadLine() ?? string.Empty;
    var filePath = path + $"/{fileName}";

    try
    {
        var doc = new XmlDocument();
        doc.Load(filePath);
        if (doc.DocumentElement == null) 
            throw new NullReferenceException("Something went wrong loading the XML file");
        ParseXml(doc.DocumentElement);
        DisplayData(fileName);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error: {e.Message}");
        GenerateStatistics();
    }
}

void ParseXml(XmlNode node)
{
    if (node == null) return;
    if (node.NodeType == XmlNodeType.Text) return;
    
    AddOrUpdateTag(node);
    
    if (IsTextNode(node)) AddOrUpdateValue(node);
    
    foreach (XmlNode child in node.ChildNodes) ParseXml(child);
}

void AddOrUpdateTag(XmlNode node)
{
    var tag = tags.Find(x => x.Name == node.Name);
    if (tag == null)
    {
        tags.Add(new Tag { Name = node.Name.Trim(), Count = 1 });
    }
    else
    {
        ++tag.Count;
    }
}

void AddOrUpdateValue(XmlNode node)
{
    var tag = tags.Find(x => x.Name == node.Name);
    var value = values.Find(x => x.Name == node.InnerText);
    var newValue = new Value { Name = Regex.Replace(node.InnerText.Trim(), @"\s+", " "), Count = 1, Tag = tag };
    if (value == null)
    {
        values.Add(newValue);
    }
    else
    {
        ++value.Count;
    }
    if (tag != null && !tag.Values.Exists(x => x.Name == value?.Name)) tag.Values.Add(newValue);
    if (tag != null) AddOrUpdateRange(node, tag);
}

void AddOrUpdateRange(XmlNode node, Tag tag)
{
    if (int.TryParse(node.InnerText.Trim(), out int age))
    {
        var existingRange = ranges.Find(x => x.Tag.Name == tag?.Name);
        if (existingRange != null)
        {
            existingRange.Numbers.Add(age);
        }
        else
        {
            ranges.Add(new Range
            {
                Numbers = new List<int>() {age},
                Tag = tag
            });
        }
    }
}

bool IsTextNode(XmlNode node)
{
    return !string.IsNullOrWhiteSpace(node.InnerText) &&
           node.ChildNodes.Count == 1 && 
           node.FirstChild?.NodeType == XmlNodeType.Text;
}

void DisplayData(string fileName)
{
    Console.Clear();
    Console.WriteLine($"{fileName}: \n");
    
    DisplayEmptyTags();
    DisplayValueTags();
    DisplayRanges();
    
    Restart();
}

void DisplayEmptyTags()
{
    var emptyTags = tags.Where(x => x.Values.Count <= 0).ToList();
    var emptyTagsTable = new ConsoleTable("Tag", "Count");
    foreach (var emptyTag in emptyTags)
    {
        emptyTagsTable.AddRow(emptyTag.Name, emptyTag.Count);
    }
    emptyTagsTable.Write(Format.Alternative);
    Console.WriteLine();
}

void DisplayValueTags()
{
    var valueTags = tags.Where(x => x.Values.Count > 0).ToList();
    foreach (var valueTag in valueTags)
    {
        var valueTagsTable = new ConsoleTable(valueTag.Name, valueTag.Count.ToString());
        foreach (var value in valueTag.Values)
        {
            valueTagsTable.AddRow(value.Name, value.Count);
        }
        valueTagsTable.Write(Format.Alternative);
        Console.WriteLine();
    }
}

void DisplayRanges()
{
    if (ranges.Any())
    {
        var ageRangeTable = new ConsoleTable("Tag", "Range");
        foreach (var range in ranges)
        {
            if (range.Numbers.Count > 1 && range.Numbers.Min() != range.Numbers.Max()) 
                ageRangeTable.AddRow(range.Tag.Name, $"{range.Numbers.Min()} – {range.Numbers.Max()}");
        }
        ageRangeTable.Write(Format.Alternative);
        Console.WriteLine();
    }
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
