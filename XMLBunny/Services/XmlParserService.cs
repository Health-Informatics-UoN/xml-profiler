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
        var tag = tags.Find(x => x.Name == node.Name);

        // ✅ Add attribute values
        AddAttributesAsValues(node, tag, values, ranges);

        if (IsTextNode(node))
        {
            AddOrUpdateValue(node, tags, values, ranges);
        }

        foreach (XmlNode child in node.ChildNodes)
        {
            ParseXml(child, tags, values, ranges);
        }
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
        var cleanValue = Regex.Replace(node.InnerText.Trim(), @"\s+", " ");
        var value = values.Find(x => x.Name == cleanValue);
        var newValue = new Value { Name = cleanValue, Count = 1, Tag = tag };

        if (value == null)
        {
            values.Add(newValue);
        }
        else
        {
            ++value.Count;
        }

        if (tag != null && !tag.Values.Exists(x => x.Name == newValue.Name))
        {
            tag.Values.Add(newValue);
        }

        if (tag != null)
        {
            AddOrUpdateRange(node.InnerText.Trim(), tag, ranges);
        }
    }

    // ✅ NEW METHOD: Adds attribute values
    private void AddAttributesAsValues(XmlNode node, Tag tag, List<Value> values, List<NumberRange> ranges)
    {
        if (node.Attributes == null) return;

        foreach (XmlAttribute attr in node.Attributes)
        {
            string attrName = $"{node.Name}.{attr.Name}";
            string attrValue = Regex.Replace(attr.Value.Trim(), @"\s+", " ");

            var value = values.Find(v => v.Name == attrValue);
            var newValue = new Value
            {
                Name = attrValue,
                Count = 1,
                Tag = tag
            };

            if (value == null)
            {
                values.Add(newValue);
            }
            else
            {
                ++value.Count;
            }

            if (tag != null && !tag.Values.Exists(x => x.Name == attrValue))
            {
                tag.Values.Add(newValue);
            }

            // ✅ Also add to ranges if it's numeric
            if (tag != null)
            {
                AddOrUpdateRange(attrValue, tag, ranges);
            }
        }
    }

    // ✅ Slightly modified to reuse for text or attribute value
    private void AddOrUpdateRange(string text, Tag tag, List<NumberRange> ranges)
    {
        if (int.TryParse(text, out int number))
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
                    Numbers = new List<int> { number },
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
