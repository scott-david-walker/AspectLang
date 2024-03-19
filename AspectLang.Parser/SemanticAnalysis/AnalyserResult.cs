namespace AspectLang.Parser.SemanticAnalysis;

public class AnalyserResult
{
    public SemanticAnalysis SemanticAnalysis { get; set; }
    public List<ParserError> Errors { get; } = [];

    public AnalyserResult(SemanticAnalysis semanticAnalysis)
    {
        SemanticAnalysis = semanticAnalysis;
    }
    public AnalyserResult(ParserError parserError)
    {
        Errors.Add(parserError);
    }
}