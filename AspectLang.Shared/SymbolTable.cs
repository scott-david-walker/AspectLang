namespace AspectLang.Shared;

public class SymbolTable
{
    private readonly Dictionary<string, Symbol> _store = new();
    
    public Symbol Define(string s)
    {
        if (Exists(s))
        {
            var symbol = Resolve(s);
            _store[s] = symbol;
            return symbol;
        }
        var newSymbol = new Symbol(s, _store.Count);
        _store[s] = newSymbol;
        return newSymbol;
    }
    public Symbol Resolve(string identifierName)
    {
        if (!Exists(identifierName))
        {
            throw new Exception("Can't find symbol");
        }
        return _store[identifierName];
    }

    public bool Exists(string identifier)
    {
        return _store.ContainsKey(identifier);
    }
}