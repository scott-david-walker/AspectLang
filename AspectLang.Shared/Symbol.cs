namespace AspectLang.Shared;

public class Symbol
{
    public string Name { get; set; }
    public SymbolScope SymbolScope { get; set; }
    public Guid ScopeId { get; set; }
    public Guid? ParentScopeId { get; set; }
}
