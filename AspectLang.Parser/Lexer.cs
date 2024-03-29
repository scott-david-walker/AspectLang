using System.Text;

namespace AspectLang.Parser;

public class Lexer
{
    private readonly string _source;
    private int _currentPosition;
    private int _readPosition;
    private char _currentChar;
    private int _currentLineNumber = 1;
    private int _currentColumnNumber = -1;

    private Dictionary<string, TokenType> _keywords = new()
    {
        { "val", TokenType.Val },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "return", TokenType.Return },
        { "fn", TokenType.Function },
        { "iterate", TokenType.Iterate },
        { "over", TokenType.Over },
        { "until", TokenType.Until },
        { "continue", TokenType.Continue },
        { "break", TokenType.Break }
    };
    public Lexer(string source)
    {
        _source = source;
        _currentChar = '\0';
        ReadNextCharacter();
    }
    
    public Token NextToken()
    {
        EatWhitespace();
        if (_currentChar == '\0')
        {
            return new(_currentLineNumber, _currentColumnNumber, TokenType.EndOfFile);
        }

        return IdentifyToken();
    }

    private Token IdentifyToken()
    {
        Token? token = null;
        if (_currentChar == '{')
        {
            token = new ("{", _currentLineNumber, _currentColumnNumber, TokenType.LeftCurly);
        }
        if (_currentChar == '}')
        {
            token = new ("}", _currentLineNumber, _currentColumnNumber, TokenType.RightCurly);
        }
        if (_currentChar == '[')
        {
            token = new ("[", _currentLineNumber, _currentColumnNumber, TokenType.LeftSquareBracket);
        }
        if (_currentChar == ']')
        {
            token = new ("]", _currentLineNumber, _currentColumnNumber, TokenType.RightSquareBracket);
        }
        if (_currentChar == ',')
        {
            token = new (",", _currentLineNumber, _currentColumnNumber, TokenType.Comma);
        }
        if (_currentChar == '"')
        {
            var startChar = _readPosition;
            while (PeekChar() != '"')
            {
                ReadNextCharacter();
            }

            var endChar = _currentPosition;
            var literal = _source.Substring(startChar, endChar - startChar + 1);
            ReadNextCharacter();
            token = new (literal, _currentLineNumber, _currentColumnNumber, TokenType.String);
        }
        if (_currentChar == '*')
        {
            token = new("*", _currentLineNumber, _currentColumnNumber, TokenType.Asterisk);
        }
        if (_currentChar == '/')
        {
            token = new("/", _currentLineNumber, _currentColumnNumber, TokenType.Slash);
        }
        if (_currentChar == '-')
        {
            token = new("-", _currentLineNumber, _currentColumnNumber, TokenType.Minus);
        }
        if (_currentChar == '+')
        {
            token = new("+", _currentLineNumber, _currentColumnNumber, TokenType.Plus);
        }
        if (_currentChar == ')')
        {
            token = new(")", _currentLineNumber, _currentColumnNumber, TokenType.RightParen);
        }
        if (_currentChar == '(')
        {
            token = new("(", _currentLineNumber, _currentColumnNumber, TokenType.LeftParen);
        }
        
        if (_currentChar == '=')
        {
            if (PeekChar() == '=')
            {
                token = new("==", _currentLineNumber, _currentColumnNumber, TokenType.Equality);
                ReadNextCharacter();
            }
            else
            {
                token = new("=", _currentLineNumber, _currentColumnNumber, TokenType.Assignment);
            }
        }
        
        if (_currentChar == '!')
        {
            if (PeekChar() == '=')
            {
                token = new("!=", _currentLineNumber, _currentColumnNumber, TokenType.NotEqual);
                ReadNextCharacter();
            }
            else
            {
                token = new("!", _currentLineNumber, _currentColumnNumber, TokenType.Exclamation);
            }
        }
        if (_currentChar == '<')
        {
            if (PeekChar() == '=')
            {
                token = new ("<=", _currentLineNumber, _currentColumnNumber, TokenType.LessThanEqualTo);
                ReadNextCharacter();
            }
            else
            {
                token = new ("<", _currentLineNumber, _currentColumnNumber, TokenType.LessThan);
            }
        }
        if (_currentChar == '>')
        {
            if (PeekChar() == '=')
            {
                token = new (">=", _currentLineNumber, _currentColumnNumber, TokenType.GreaterThanEqualTo);
                ReadNextCharacter();
            }
            else
            {
                token = new (">", _currentLineNumber, _currentColumnNumber, TokenType.GreaterThan);
            }
        }
        if (_currentChar == ';')
        {
            token = new(";", _currentLineNumber, _currentColumnNumber, TokenType.SemiColon);
        }
        
        if (IsDigit(_currentChar))
        {
            var startColumn = _currentColumnNumber;
            var sb = new StringBuilder();
            sb.Append(_currentChar);
            while (char.IsDigit(PeekChar()))
            {
                ReadNextCharacter();
                sb.Append(_currentChar);
            }
            token =  new(sb.ToString(), _currentLineNumber, startColumn, TokenType.Integer);
        }
        if (char.IsLetter(_currentChar))
        {
            var startColumn = _currentColumnNumber;
            var sb = new StringBuilder();
            while (IsIdentifierCompliant(_currentChar))
            {
                sb.Append(_currentChar);
                if (!IsIdentifierCompliant(PeekChar()))
                {
                    break;
                }
                ReadNextCharacter();
            }

            var identifier = sb.ToString();
            if (_keywords.TryGetValue(identifier, out TokenType value))
            {
                token =  new(identifier, _currentLineNumber, startColumn, value);
            }
            else
            {
                token = new(sb.ToString(), _currentLineNumber, startColumn, TokenType.Identifier);
            }
        }

        if (token == null)
        {
            token = new(_currentLineNumber, _currentColumnNumber, TokenType.Illegal);
        }

        ReadNextCharacter();
        return token;
    }

    private bool IsIdentifierCompliant(char character)
    {
        return IsLetter(character) || IsDigit(character) || character == '_';
    }

    private char PeekChar()
    {
        if (_readPosition >= _source.Length)
        {
            return '\0';
        }
        return _source[_readPosition];
    }

    private static bool IsDigit(char currentChar)
    {
        return char.IsDigit(currentChar);
    }

    private static bool IsLetter(char currentChar)
    {
        return char.IsLetter(currentChar);
    }

    private void EatWhitespace()
    {
        if (_currentChar == '/' && PeekChar() == '/')
        {
            while (_currentChar != '\n' && _currentChar != '\0')
            {
                ReadNextCharacter();
            }
        }
        if (_currentChar == '\n')
        {
            _currentLineNumber++;
            _currentColumnNumber = 0;
            ReadNextCharacter();
            EatWhitespace();
        }

        if (_currentChar == '\t')
        {
            _currentColumnNumber += 3; // next character will bump it to 4
            ReadNextCharacter();
            EatWhitespace();
        }
        if (_currentChar == '\r')
        {
            ReadNextCharacter();
            EatWhitespace();
        }

        while (_currentChar == 32)
        {
            ReadNextCharacter();
            EatWhitespace();
        }
    }
    private void ReadNextCharacter()
    {
        _currentColumnNumber++;
        if (_readPosition >= _source.Length)
        {
            _currentChar = '\0';
        }
        else
        {
            _currentChar = _source[_readPosition];
        }

        _currentPosition = _readPosition;
        _readPosition++;
    }
}