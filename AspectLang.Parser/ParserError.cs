namespace AspectLang.Parser;

public class ParserError
{
    public string Message { get; }
    public int LineNumber { get; }
    public int ColumnPosition { get; }

    public ParserError(string message, Token token)
    {
        Message = message;
        LineNumber = token.LineNumber;
        ColumnPosition = token.ColumnPosition;
    }
    
    public ParserError(string message, int lineNumber, int columnPosition)
    {
        Message = message;
        LineNumber = lineNumber;
        ColumnPosition = columnPosition;
    }
}