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

        // Ensure all attributes are processed and stored using namespaced keys for clarity
        if (tag != null)
        {
            AddAttributesAsValues(node, tag, tags, values, ranges);
        }

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
        var value = values.Find(x => x.Name == cleanValue && x.Tag == tag);
        var newValue = new Value { Name = cleanValue, Count = 1, Tag = tag };

        if (value == null)
        {
            values.Add(newValue);
        }
        else
        {
            ++value.Count;
        }

        if (tag != null && !tag.Values.Exists(x => x.Name == cleanValue))
        {
            tag.Values.Add(newValue);
        }

        if (tag != null)
        {
            AddOrUpdateRange(cleanValue, tag, ranges);
        }
    }

    // Improved: Ensures attributes are processed and stored using namespaced keys for better clarity and organization.
    private void AddAttributesAsValues(XmlNode node, Tag tag, List<Tag> tags, List<Value> values, List<NumberRange> ranges)
    {
        if (node.Attributes == null) return;

        foreach (XmlAttribute attr in node.Attributes)
        {
            string namespacedName = $"{node.Name}.{attr.Name}";
            string cleanValue = Regex.Replace(attr.Value.Trim(), @"\s+", " ");

            var attrTag = tags.Find(x => x.Name == namespacedName);
            if (attrTag == null)
            {
                attrTag = new Tag { Name = namespacedName, Count = 1 };
                tags.Add(attrTag);
            }
            else
            {
                ++attrTag.Count;
            }

            var value = values.Find(v => v.Name == cleanValue && v.Tag == attrTag);
            var newValue = new Value
            {
                Name = cleanValue,
                Count = 1,
                Tag = attrTag
            };

            if (value == null)
            {
                values.Add(newValue);
            }
            else
            {
                ++value.Count;
            }

            if (!attrTag.Values.Exists(x => x.Name == cleanValue))
            {
                attrTag.Values.Add(newValue);
            }

            AddOrUpdateRange(cleanValue, attrTag, ranges);
        }
    }

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
