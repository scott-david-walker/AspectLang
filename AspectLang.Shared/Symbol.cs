namespace AspectLang.Shared;

public class Symbol(string name, SymbolScope scope, int index)
{
    public string Name { get; set; } = name;
    public SymbolScope Scope { get; set; } = scope;
    public int Index {get; set; } = index;
}