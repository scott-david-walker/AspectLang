namespace AspectLang.Parser;

public class Token
{
    public string Literal { get; set; }
    public string FileName { get; set; }
    public int LineNumber { get; set; }
    public int ColumnPosition { get; set; }
    public TokenType TokenType { get; set; }

    public Token(string literal, int lineNumber, int columnPosition, TokenType tokenType)
    {
        Literal = literal;
        LineNumber = lineNumber;
        ColumnPosition = columnPosition;
        TokenType = tokenType;
    }
    public Token(int lineNumber, int columnPosition, TokenType tokenType)
    {
        LineNumber = lineNumber;
        ColumnPosition = columnPosition;
        TokenType = tokenType;
    }
}