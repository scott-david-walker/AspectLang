using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.SemanticAnalysis;

namespace AspectLang.Parser;

public class Parser
{
    private readonly Dictionary<TokenType, Priority> _precendences = new()
    {
        { TokenType.LeftBracket, Priority.Index },
        { TokenType.Equality, Priority.Equals },
        { TokenType.NotEqual, Priority.Equals },
        { TokenType.LessThan, Priority.LessGreater },
        { TokenType.GreaterThan, Priority.LessGreater },
        { TokenType.LessThanEqualTo, Priority.LessGreater },
        { TokenType.GreaterThanEqualTo, Priority.LessGreater },
        { TokenType.Plus, Priority.Sum },
        { TokenType.Minus, Priority.Sum },
        { TokenType.Slash, Priority.Product },
        { TokenType.Asterisk, Priority.Product },
        { TokenType.LeftParen, Priority.FunctionCall }

    };
    private readonly Dictionary<TokenType, Func<IExpression>> _prefixParseFunctions = new();
    private readonly Dictionary<TokenType, Func<IExpression, IExpression>> _infixParseFunctions = new();
    private readonly Lexer _lexer;
    private Token _currentToken;
    private Token _previousToken;
    private Token _peekToken;

    public Parser(Lexer lexer)
    {
        _prefixParseFunctions.Add(TokenType.Identifier, ParseIdentifier);
        _prefixParseFunctions.Add(TokenType.Integer, ParseIntegerExpression);
        _prefixParseFunctions.Add(TokenType.String, ParseStringExpression);
        _prefixParseFunctions.Add(TokenType.LeftParen, ParseGroupedExpression);
        _prefixParseFunctions.Add(TokenType.Minus, ParsePrefixExpression);
        _prefixParseFunctions.Add(TokenType.Exclamation, ParsePrefixExpression);
        _prefixParseFunctions.Add(TokenType.False, ParseBooleanExpression);
        _prefixParseFunctions.Add(TokenType.True, ParseBooleanExpression);

        _infixParseFunctions.Add(TokenType.Plus, ParseInfixExpression);
        _infixParseFunctions.Add(TokenType.Minus, ParseInfixExpression);
        _infixParseFunctions.Add(TokenType.Slash, ParseInfixExpression);
        _infixParseFunctions.Add(TokenType.Asterisk, ParseInfixExpression);
        _infixParseFunctions.Add(TokenType.Equality, ParseInfixExpression);
        _infixParseFunctions.Add(TokenType.NotEqual, ParseInfixExpression);
        _infixParseFunctions.Add(TokenType.LessThan, ParseInfixExpression);
        _infixParseFunctions.Add(TokenType.GreaterThan, ParseInfixExpression);
        _infixParseFunctions.Add(TokenType.LessThanEqualTo, ParseInfixExpression);
        _infixParseFunctions.Add(TokenType.GreaterThanEqualTo, ParseInfixExpression);
        _infixParseFunctions.Add(TokenType.LeftParen, ParseFunctionCall);
        _lexer = lexer;
        GetNext();
        GetNext();
    }

    private IExpression ParseFunctionCall(IExpression identifierExpression)
    {
        var identifier = identifierExpression as Identifier;
        var call = new FunctionCall
        {
            Name = identifier.Name,
            Token = _currentToken
        };
        call.Args = ParseExpressionList(TokenType.RightParen);
        return call;
    }

    private List<IExpression> ParseExpressionList(TokenType token)
    {
        var list = new List<IExpression>();
        if (_peekToken.TokenType == token)
        {
            GetNext();
            return list;
        }
        GetNext();
        list.Add(ParseExpression(Priority.Lowest));
        while (_peekToken.TokenType == TokenType.Comma)
        { 
            GetNext();   
            GetNext();
            list.Add(ParseExpression(Priority.Lowest));
        }

        AssertNextToken(token);

        return list;
    }
    private Identifier ParseIdentifier()
    {
        return new()
        {
            Token = _currentToken,
            Name = _currentToken.Literal
        };
    }

    private BlockStatement ParseBlockStatement()
    {
        var nested = 0;
        AssertNextToken(TokenType.LeftCurly);
        if (_peekToken.TokenType == TokenType.RightCurly)
        {
            throw new ParserException("Empty block statements are not allowed", _peekToken);
        }
        GetNext();

        var block = new BlockStatement();
        while (_peekToken.TokenType != TokenType.RightCurly)
        {
            var statement = ParseStatement();
            if (statement is IfStatement or BlockStatement)
            {
                nested++;
            }
            block.Statements.Add(statement);
            if (_peekToken.TokenType == TokenType.SemiColon)
            {
                GetNext();
            }
            // if (_currentToken.TokenType == TokenType.RightCurly)
            // {
            //     GetNext();
            // }
        }
        AssertNextToken(TokenType.RightCurly);
        return block;
    }

    private IExpression ParseBooleanExpression()
    {
        return _currentToken.TokenType == TokenType.True ? new (true, _currentToken) : new BooleanExpression(false, _currentToken);
    }

    private IExpression ParsePrefixExpression()
    { 
        var expression = new PrefixExpression(_currentToken);
        GetNext();
        expression.Right = ParseExpression(Priority.Prefix);
        return expression;
    }

    private IExpression ParseGroupedExpression()
    {
        GetNext();
        var expression = ParseExpression(Priority.Lowest);
        AssertNextToken(TokenType.RightParen);
        return expression;
    }

    private IExpression ParseInfixExpression(IExpression expression)
    {
        var infixExpression =  new InfixExpression
        {
            Operator = _currentToken.Literal,
            Left = expression,
            Token = _currentToken
        };
        
        var precedence = CurrentPrecedence();
        GetNext();
        infixExpression.Right = ParseExpression(precedence);
        return infixExpression;
    }

    private ReturnStatement ParseReturnStatement()
    {
        GetNext();

        var expression = ParseExpression(Priority.Lowest);

        var statement = new ReturnStatement
        {
            Token = _currentToken,
            Value = expression
        };
        AssertNextToken(TokenType.SemiColon);
        return statement;
    }
    private VariableAssignmentNode ParseValAssignment(bool isFreshAssignment)
    {
        var variable = new VariableDeclarationNode(_currentToken, isFreshAssignment);
        if (_peekToken.TokenType != TokenType.Assignment)
        {
            throw new ParserException($"Expected = but received {_peekToken.Literal}", _peekToken);
        }

        GetNext();
        GetNext();
        var expression = ParseExpression(Priority.Lowest);
        var assignmentNode = new VariableAssignmentNode(variable, expression, _currentToken);
        AssertNextToken(TokenType.SemiColon);
        return assignmentNode;
    }
    private VariableAssignmentNode ParseValStatement()
    {
        if (_peekToken.TokenType != TokenType.Identifier)
        {
            throw new ParserException($"Expected identifier but received {_peekToken.Literal}", _peekToken);
        }

        GetNext();
        return ParseValAssignment(true);
    }
    private IStatement ParseStatement()
    {
        if (_currentToken.TokenType == TokenType.SemiColon || _currentToken.TokenType == TokenType.RightCurly)
        {
            GetNext();
        }
        switch (_currentToken.TokenType)
        {
            case TokenType.Val:
                return ParseValStatement();
            case TokenType.Identifier when _peekToken.TokenType != TokenType.LeftParen:
                return ParseValAssignment(false);
            case TokenType.Return:
                return ParseReturnStatement();
            case TokenType.If:
                return ParseIfStatement();
            case TokenType.Function:
                return ParseFunction();
        }

        return ParseExpressionStatement();
    }

    private FunctionDeclarationStatement ParseFunction()
    {
        var statement = new FunctionDeclarationStatement();
        statement.Token = _currentToken;
        AssertNextToken(TokenType.Identifier);
        statement.Name = _currentToken.Literal;
        AssertNextToken(TokenType.LeftParen);
        while (_peekToken.TokenType != TokenType.RightParen)
        {
            GetNext();
            var argument = ParseIdentifier();
            statement.Parameters.Add(argument);
            if (_peekToken.TokenType != TokenType.RightParen)
            {
                AssertNextToken(TokenType.Comma);
            }
        }
        AssertNextToken(TokenType.RightParen);
        statement.Body = ParseBlockStatement();
        return statement;
    }
    private IfStatement ParseIfStatement()
    {
        AssertNextToken(TokenType.LeftParen);

        GetNext();
        var condition = ParseExpression(Priority.Lowest);
        AssertNextToken(TokenType.RightParen);
        var consequence = ParseBlockStatement();
        BlockStatement? alternative = null;
        if (IsNextToken(TokenType.Else))
        {
            GetNext();
            alternative = ParseBlockStatement();
        }
        return new()
        {
            Condition = condition,
            Consequence = consequence,
            Alternative = alternative
        };
    }

    public ParseResult Parse()
    {
        var parseResult = new ParseResult();
        try
        {
            while (_peekToken.TokenType != TokenType.EndOfFile)
            {
                var statement = ParseStatement();
                parseResult.ProgramNode.StatementNodes.Add(statement);
                GetNext();
            }
            new Analyser().Analyse(parseResult.ProgramNode);
        }
        catch (ParserException ex)
        {
            return new(new ParserError(ex.Message, ex.LineNumber, ex.ColumnNumber));
        }

        
        return parseResult;
    }
    
    

    private void GetNext()
    {
        _previousToken = _currentToken;
        _currentToken = _peekToken;
        _peekToken = _lexer.NextToken();
    }

    private IStatement ParseExpressionStatement()
    {
        var statement = new ExpressionStatement();

        statement.Expression = ParseExpression(Priority.Lowest);
        if (IsNextToken(TokenType.SemiColon))
        {
            GetNext();
        }

        return statement;
    }
    
    private IExpression ParseExpression(Priority priority)
    {
        if (!_prefixParseFunctions.ContainsKey(_currentToken.TokenType))
        {
            throw new ParserException($"Unable to parse expression {_currentToken.Literal}",
                _currentToken.LineNumber, _currentToken.ColumnPosition);
            
        }
        var prefix = _prefixParseFunctions[_currentToken.TokenType];
        var expression = prefix();
        while (_peekToken.TokenType != TokenType.SemiColon && priority < PeekPrecedence())
        {
            if (!_infixParseFunctions.ContainsKey(_peekToken.TokenType))
            {
                return expression;
            }
            var infix = _infixParseFunctions[_peekToken.TokenType];

            GetNext();
            expression = infix(expression);
        }
        return expression;
    }
    
    private Priority PeekPrecedence()
    {
        if (!_precendences.ContainsKey(_peekToken.TokenType))
        {
            return Priority.Lowest;
        }
        var priority =  _precendences[_peekToken.TokenType];
        return priority;
    }

    private void AssertNextToken(TokenType tokenType)
    {
        if (_peekToken.TokenType != tokenType)
        {
            throw new ParserException($"Expected {tokenType} but got {_peekToken.TokenType}", _peekToken);
        }
        GetNext();
    }

    private bool IsNextToken(TokenType tokenType)
    {
        return _peekToken.TokenType == tokenType;
    }
    
    private Priority CurrentPrecedence()
    {
        if (!_precendences.ContainsKey(_currentToken.TokenType))
        {
            return Priority.Lowest;
        }
        var priority =  _precendences[_currentToken.TokenType];
        return priority;
    }
    private IExpression ParseIntegerExpression()
    {
        var token = _currentToken;
        if (int.TryParse(token.Literal, out var value))
        {
            return new IntegerExpression(value, _currentToken);
        }

        throw new ParserException($"Unable to parse {token.Literal} as string", _currentToken.LineNumber,
            _currentToken.ColumnPosition);
    }
    
    private IExpression ParseStringExpression()
    {
        return new StringExpression(_currentToken.Literal);
    }
}

internal class ParserException : Exception
{
    public int LineNumber { get; }
    public int ColumnNumber { get; }
    public string Message { get; }

    public ParserException(string message, Token token)
    {
        Message = message;
        LineNumber = token.LineNumber;
        ColumnNumber = token.ColumnPosition;
    }
    public ParserException(string message, int lineNumber, int columnNumber)
    {
        Message = message;
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }
}