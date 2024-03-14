namespace AspectLang.Parser;

public class ParserError(string message, int lineNumber, int columnPosition)
{
    public string Message { get; } = message;
    public int LineNumber { get; } = lineNumber;
    public int ColumnPosition { get; } = columnPosition;
}