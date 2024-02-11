namespace AspectLang.Parser.Ast;

public class ParseResult
{
    public ProgramNode ProgramNode { get; } = new();
    public List<ParserError> Errors { get; } = [];

    public ParseResult()
    {
        
    }
    public ParseResult(ProgramNode programNode)
    {
        ProgramNode = programNode;
    }
    
    public ParseResult(ParserError parserError)
    {
        Errors.Add(parserError);
    }
}