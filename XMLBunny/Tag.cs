namespace XMLBunny;

public class Tag
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; } = 0;
    public List<Value> Values { get; set; } = new ();
}
