namespace AspectLang.Shared;

public class SymbolTable
{
    private readonly Dictionary<string, Symbol> _store = new();

    public Symbol Define(string s, SymbolScope symbolScope)
    {
        var symbol = new Symbol(s, symbolScope, _store.Count);
        _store[s] = symbol;
        return symbol;
    }
    public Symbol Define(string s)
    {
        var symbol = new Symbol(s, SymbolScope.Global, _store.Count);
        _store[s] = symbol;
        return symbol;
    }
    public Symbol Resolve(string identifierName)
    {
        return _store[identifierName];
    }

    public bool Exists(string identifier)
    {
        return _store.ContainsKey(identifier);
    }
}