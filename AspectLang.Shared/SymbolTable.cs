namespace AspectLang.Shared;

public class SymbolTable
{
    public List<Symbol> Symbols { get; set; } = [];
    
    public Symbol Define(string name, Guid scopeId, SymbolScope symbolScope)
    {
        return Define(name, scopeId, symbolScope, null);
    }
    
    public Symbol Define(string name, Guid scopeId, SymbolScope symbolScope, Guid? parentScopeId)
    {
        var symbol = new Symbol
        {
            Name = name,
            SymbolScope = symbolScope,
            ScopeId = scopeId,
            ParentScopeId = parentScopeId
        };
        Symbols.Add(symbol);
        return symbol;
    }
    public Symbol? Resolve(string identifier, Guid scopeId)
    {
        if (!Exists(identifier, scopeId))
        {
            return null;
        }

        return Symbols.First(t => t.Name == identifier && t.ScopeId == scopeId);
    }

    public bool Exists(string identifier, Guid scopeId)
    {
        return Symbols.FirstOrDefault(t => t.Name == identifier && t.ScopeId == scopeId) != null;
    }
}