namespace AspectLang.Shared;

public class Scope
{
    public Scope(Scope? parent)
    {
        Parent = parent;
    }
    public SymbolTable SymbolTable { get; set; } = new();
    public Scope? Parent { get; set; }
}