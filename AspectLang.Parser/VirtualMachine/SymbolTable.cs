namespace AspectLang.Parser.VirtualMachine;

public class SymbolTable
{
    private readonly Dictionary<string, Symbol> _store = new();

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
}