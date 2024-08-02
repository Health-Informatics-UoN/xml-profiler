using System.Text.RegularExpressions;
using System.Xml;
using ConsoleTables;
using XMLBunny;

var tags = new List<Tag>();
var values = new List<Value>();
var ages = new List<int>();

void GenerateStatistics()
{ 
    // Get the file path from Desktop
    var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    Console.Write("Enter XML File Name: ");
    var fileName = Console.ReadLine() ?? "";
    var filePath = path + $"/{fileName}";

    try
    {
        var doc = new XmlDocument();
        doc.Load(filePath);
        var root = doc.DocumentElement;
        
        ParseXml(root);
        DisplayData(fileName);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error: {e.Message}");
        
        // Recursive call in the case where there is an error
        GenerateStatistics();
    }
}

void ParseXml(XmlNode node)
{
    if (node == null) return;
    if (node.NodeType == XmlNodeType.Text) return;
    
    // Check if tag already exists. If it does, increase count, if not create new tag
    var tag = tags.Find(x => x.Name == node.Name);
    if (tag == null)
    {
        tags.Add(new Tag { Name = node.Name.Trim(), Count = 1 });
    }
    else
    {
        ++tag.Count;
    }
    
    if (!string.IsNullOrWhiteSpace(node.InnerText) && node.ChildNodes.Count == 1 && node.FirstChild.NodeType == XmlNodeType.Text)
    {
        // Check if the value exists and increase if it does. Create new value if it doesn't and attach to a tag
        var t = tags.Find(x => x.Name == node.Name);
        var value = values.Find(x => x.Name == node.InnerText);
        var newValue = new Value { Name = Regex.Replace(node.InnerText.Trim(), @"\s+", " "), Count = 1, Tag = t };
        if (value == null)
        {
            values.Add(newValue);
        }
        else
        {
            ++value.Count;
        }
        if (t != null && !t.Values.Exists(x => x.Name == value?.Name)) t.Values.Add(newValue);
        
        // Update the ages
        if (node.Name.ToLower().Trim() == "age" && int.TryParse(node.InnerText.Trim(), out int age))
        {
            ages.Add(age);
        }
    }
    
    foreach (XmlNode child in node.ChildNodes)
    {
        ParseXml(child);
    }
}

void DisplayData(string fileName)
{
    Console.Clear();
    Console.WriteLine($"{fileName}: \n");
    
    var emptyTags = tags.Where(x => x.Values.Count <= 0).ToList();
    var emptyTagsTable = new ConsoleTable("Tag", "Count");
    foreach (var emptyTag in emptyTags)
    {
        emptyTagsTable.AddRow(emptyTag.Name, emptyTag.Count);
    }
    emptyTagsTable.Write();
    Console.WriteLine();
    
    var valueTags = tags.Where(x => x.Values.Count > 0).ToList();
    foreach (var valueTag in valueTags)
    {
        var valueTagsTable = new ConsoleTable(valueTag.Name, valueTag.Count.ToString());
        foreach (var value in valueTag.Values)
        {
            valueTagsTable.AddRow(value.Name, value.Count);
        }
        valueTagsTable.Write();
        Console.WriteLine();
    }
    
    var ageRangeTable = new ConsoleTable("Age Range");
    ageRangeTable.AddRow($"{ages.Min()} – {ages.Max()}");
    ageRangeTable.Write();
    Console.WriteLine();
    
    // Restart after displaying data
    Restart();
}

void Restart()
{
    Console.Write("Do you want to parse another file?: ");
    var response = Console.ReadLine() ?? "";
    if (response.ToLower().Trim() == "yes")
    {
        GenerateStatistics();
    } else if (response.ToLower().Trim() == "no")
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
