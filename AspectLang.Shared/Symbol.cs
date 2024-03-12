namespace AspectLang.Shared;

public class Symbol(string name, int index)
{
    public string Name { get; set; } = name;
    public int Index {get; set; } = index;
}