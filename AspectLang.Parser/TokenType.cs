namespace AspectLang.Parser;

public enum TokenType
{
    Illegal,
    Identifier,
    Assignment,
    Val,
    EndOfLine,
    EndOfFile,
    Integer,
    SemiColon,
    LeftBracket,
    Equality,
    NotEqual,
    LessThan,
    GreaterThan,
    Plus,
    Minus,
    Slash,
    Asterisk,
    LeftParen,
    RightParen
}