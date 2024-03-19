using AspectLang.Shared;

namespace AspectLang.Parser.SemanticAnalysis;

public class SemanticAnalysis
{
    public Dictionary<Guid, Guid?> Scopes { get; set; }
    public SymbolTable SymbolTable { get; set; }
    public List<Guid> EnterScopes { get; set; }
}