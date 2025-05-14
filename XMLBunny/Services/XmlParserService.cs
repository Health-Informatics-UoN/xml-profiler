using System.Text.RegularExpressions;
using System.Xml;
using XMLBunny.Models;

namespace XMLBunny.Services;

public class XmlParserService
{
    /// <summary>
    /// Parses an XML node and its children, updating the provided lists of tags, values, and number ranges.
    /// </summary>
    /// <param name="node">The XML node to process.</param>
    /// <param name="tags">A collection of tags to be updated or added to.</param>
    /// <param name="values">A collection of values to be updated or added to.</param>
    /// <param name="ranges">A collection of number ranges to be updated or added to.</param>
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

    /// <summary>
    /// Adds a new tag to the list or updates the count of an existing tag.
    /// </summary>
    /// <param name="node">The XML node representing the tag.</param>
    /// <param name="tags">The list of tags to update.</param>
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

    /// <summary>
    /// Adds a new value to the list or updates the count of an existing value.
    /// Also associates the value with its parent tag and updates numerical ranges if applicable.
    /// </summary>
    /// <param name="node">The XML node containing the value.</param>
    /// <param name="tags">The list of tags to update.</param>
    /// <param name="values">The list of values to update.</param>
    /// <param name="ranges">The list of number ranges to update.</param>
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

    /// <summary>
    /// Processes attributes of an XML node and stores them as values associated with their parent tag.
    /// </summary>
    /// <param name="node">The XML node containing attributes.</param>
    /// <param name="tag">The parent tag to associate the attributes with.</param>
    /// <param name="tags">The list of tags to update.</param>
    /// <param name="values">The list of values to update.</param>
    /// <param name="ranges">The list of number ranges to update.</param>
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
    /// <summary>
    /// Adds a number to an existing range or creates a new range for the tag.
    /// </summary>
    /// <param name="text">The text to parse as a number.</param>
    /// <param name="tag">The tag associated with the range.</param>
    /// <param name="ranges">The list of number ranges to update.</param>
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

    /// <summary>
    /// Determines if an XML node is a text node.
    /// </summary>
    /// <param name="node">The XML node to check.</param>
    /// <returns>True if the node is a text node; otherwise, false.</returns>
    private bool IsTextNode(XmlNode node)
    {
        return !string.IsNullOrWhiteSpace(node.InnerText) &&
               node.ChildNodes.Count == 1 &&
               node.FirstChild?.NodeType == XmlNodeType.Text;
    }
}
