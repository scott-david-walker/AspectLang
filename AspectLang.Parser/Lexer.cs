using System.Text;

namespace AspectLang.Parser;

public class Lexer
{
    private readonly string _source;
    private int _currentPosition;
    private int _readPosition;
    private char _currentChar;
    private int _currentLineNumber;
    private int _currentColumnNumber = -1;

    private Dictionary<string, TokenType> _keywords = new()
    {
        { "val", TokenType.Val },
        { "true", TokenType.True },
        { "false", TokenType.False }
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
            if (_source[_readPosition] == '=')
            {
                token = new("==", _currentLineNumber, _currentColumnNumber, TokenType.Equality);
            }
            else
            {
                token = new("=", _currentLineNumber, _currentColumnNumber, TokenType.Assignment);
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
            while (IsLetter(_currentChar) || IsDigit(_currentChar) || _currentChar == '_')
            {
                sb.Append(_currentChar);
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

    private char PeekChar()
    {
        if (_readPosition >= _source.Length)
        {
            return '\0';
        }
        return _source[_readPosition];
    }

    private bool IsDigit(char currentChar)
    {
        return char.IsDigit(currentChar);
    }

    private bool IsLetter(char currentChar)
    {
        return char.IsLetter(currentChar);
    }

    private void EatWhitespace()
    {
        if (_currentChar == '\n')
        {
            _currentLineNumber++;
            _currentColumnNumber = 0;
            ReadNextCharacter();
        }

        if (_currentChar == '\t')
        {
            _currentColumnNumber += 3; // next character will bump it to 4
            ReadNextCharacter();
        }
        if (_currentChar == '\r')
        {
            _currentLineNumber++;
            _currentColumnNumber = 0;
            ReadNextCharacter();
            if (_currentChar == '\n')
            {
                // do nothing as we've already handled.
            }
        }
        
        if (_currentChar == 32)
        {
            ReadNextCharacter();
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