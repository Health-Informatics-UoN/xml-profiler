using System.Text.RegularExpressions;
using System.Xml;
using XMLBunny.Models;

namespace XMLBunny.Services;

public class XmlParserService
{
    public void ParseXml(XmlNode node, List<Tag> tags, List<Value> values, List<NumberRange> ranges)
    {
        if (node == null) return;
        if (node.NodeType == XmlNodeType.Text) return;
    
        AddOrUpdateTag(node, tags);
    
        if (IsTextNode(node)) AddOrUpdateValue(node, tags, values, ranges);
    
        foreach (XmlNode child in node.ChildNodes) ParseXml(child, tags, values, ranges);
    }
    
    private void AddOrUpdateTag(XmlNode node, List<Tag> tags)
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
    
    private void AddOrUpdateValue(XmlNode node, List<Tag> tags, List<Value> values, List<NumberRange> ranges)
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
        if (tag != null) AddOrUpdateRange(node, tag, ranges);
    }
    
    private void AddOrUpdateRange(XmlNode node, Tag tag, List<NumberRange> ranges)
    {
        if (int.TryParse(node.InnerText.Trim(), out int number))
        {
            var existingRange = ranges.Find(x => x.Tag.Name == tag?.Name);
            if (existingRange != null)
            {
                existingRange.Numbers.Add(number);
            }
            else
            {
                ranges.Add(new NumberRange
                {
                    Numbers = new List<int>() {number},
                    Tag = tag
                });
            }
        }
    }

    private bool IsTextNode(XmlNode node)
    {
        return !string.IsNullOrWhiteSpace(node.InnerText) &&
               node.ChildNodes.Count == 1 && 
               node.FirstChild?.NodeType == XmlNodeType.Text;
    }
}
