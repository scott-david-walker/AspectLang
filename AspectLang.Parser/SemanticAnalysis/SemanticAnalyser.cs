using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Ast.Statements;
using AspectLang.Parser.Compiler;
using AspectLang.Shared;

namespace AspectLang.Parser.SemanticAnalysis;

public class SemanticAnalyser : IVisitor
{
    private SymbolTable _symbolTable = new();
    private Guid _scopeId = Guid.Parse("2d54b924-5671-408a-8e3d-8d7a25b2043a");
    private readonly List<Guid> _enterScopes = [];
    private Dictionary<Guid, Guid?> _scopes { get; set; } = [];
    private readonly List<FunctionCall> _functionCalls = [];
    private readonly List<FunctionDeclarationStatement> _functions = [];
    public AnalyserResult Analyse(INode node)
    {
        try
        {
            _scopes.Add(_scopeId, null);
            node.Accept(this);
            AnalyseFunctionDeclarations();
            AnalyseFunctionCalls();
            var res = new SemanticAnalysis()
            {
                SymbolTable = _symbolTable,
                EnterScopes = _enterScopes,
                Scopes = _scopes
            };
            return new(res);
        }
        catch (ParserException ex)
        {
            return new (new ParserError(ex.Message, ex.LineNumber, ex.ColumnNumber));
        }
    }

    private void AnalyseFunctionDeclarations()
    {
        foreach (var functionDeclaration in _functions)
        {
            var scope = _scopeId;
            _scopeId = Guid.NewGuid();
            _scopes.Add(_scopeId, null);
            var scopesCount = _enterScopes.Count;
            _symbolTable.Define(functionDeclaration.Name, _scopeId, SymbolScope.Function);

            foreach (var parameter in functionDeclaration.Parameters)
            {
                _symbolTable.Define(parameter.Name, _scopeId, SymbolScope.Local);
                parameter.Accept(this);
            }

            functionDeclaration.Body.Accept(this);
            var exitScope = _enterScopes.Count;
            functionDeclaration.EntryScope = scopesCount;
            functionDeclaration.ExitScope = exitScope;
            _scopeId = scope;
        }
    }

    private void AnalyseFunctionCalls()
    {
        foreach (var functionCall in _functionCalls)
        {
           var symbol = FindSymbol(functionCall.Name, SymbolScope.Function);
           if (symbol == null)
           {
               throw new ("Function not found or something");
           }
        }
    }
    public void Visit(IntegerExpression expression)
    {
    }

    public void Visit(ProgramNode node)
    {
        foreach (var statement in node.StatementNodes)
        {
            statement.Accept(this);
        }
    }

    public void Visit(ExpressionStatement expression)
    {
    }

    public void Visit(InfixExpression expression)
    {
        expression.Left.Accept(this);
        expression.Right.Accept(this);
    }

    public void Visit(PrefixExpression expression)
    {
    }

    public void Visit(BooleanExpression expression)
    {
    }

    public void Visit(StringExpression expression)
    {
    }

    public void Visit(BlockStatement blockStatement)
    {
        EnterScope();
        foreach (var statement in blockStatement.Statements)
        {
            statement.Accept(this);
        }
        ExitScope();
    }

    public void Visit(IfStatement ifStatement)
    {
        ifStatement.Condition.Accept(this);
        ifStatement.Consequence.Accept(this);
        if (ifStatement.Alternative != null)
        {
            ifStatement.Alternative.Accept(this);
        }
    }

    public void Visit(VariableDeclarationNode variableDeclaration)
    {
    }

    public void Visit(VariableAssignmentNode variableAssignment)
    {
        variableAssignment.Expression.Accept(this);
        var symbol = FindSymbol(variableAssignment.VariableDeclarationNode.Name);
        if (symbol != null)
        {
            if (variableAssignment.VariableDeclarationNode.IsFreshDeclaration)
            {
                throw new ParserException(
                    $"Variable with name {variableAssignment.VariableDeclarationNode.Name} already exists",
                    variableAssignment.Token);
            }
        }
        else
        {
            if (variableAssignment.VariableDeclarationNode.IsFreshDeclaration)
            {
                var parentScope = _scopes.TryGetValue(_scopeId, out var scope) ? scope : null;
                _symbolTable.Define(variableAssignment.VariableDeclarationNode.Name, _scopeId, SymbolScope.Local, parentScope);
            }
            else
            {
                throw new ParserException(
                $"Variable with name {variableAssignment.VariableDeclarationNode.Name} does not exist",
                    variableAssignment.Token);
            }
        }
    }

    public void Visit(Identifier identifier)
    {
        var symbol = FindSymbol(identifier.Name);

        if (symbol == null)
        {
            throw new ParserException(
                $"Variable with name {identifier.Name} does not exist",
                identifier.Token);
        }
    }

    public void Visit(ReturnStatement returnStatement)
    {
        returnStatement.Value.Accept(this);
    }

    public void Visit(FunctionDeclarationStatement functionDeclaration)
    {
        _functions.Add(functionDeclaration);
    }

    public void Visit(FunctionCall functionCall)
    {
        foreach (var arg in functionCall.Args)
        {
            arg.Accept(this);
        }
        _functionCalls.Add(functionCall);
    }

    public void Visit(ArrayLiteral array)
    {
        foreach (var element in array.Elements)
        {
            element.Accept(this);
        }
    }

    public void Visit(IndexExpression indexExpression)
    {
        indexExpression.Left.Accept(this);
        indexExpression.Index.Accept(this);
    }

    public void Visit(IterateOverStatement iterateOver)
    {
        iterateOver.Identifier.Accept(this);
        _symbolTable.Define("it", _scopeId, SymbolScope.Local, _scopes.GetValueOrDefault(_scopeId));
        _symbolTable.Define("index", _scopeId, SymbolScope.Local, _scopes.GetValueOrDefault(_scopeId));
        iterateOver.Body.Accept(this);
    }

    public void Visit(IterateUntilStatement iterateUntil)
    {
        iterateUntil.Condition.Accept(this);
        iterateUntil.Body.Accept(this);
    }

    public void Visit(ContinueStatement continueStatement)
    {
        
    }

    private void EnterScope()
    {
        var scopeId = Guid.NewGuid();
        _enterScopes.Add(scopeId);
        _scopes.Add(scopeId, _scopeId);
        _scopeId = scopeId;
    }

    private void ExitScope()
    {
        var scope = _scopes[_scopeId];
        _scopeId = scope.Value;
    }

    private Symbol? FindSymbol(string name)
    {
        Guid? scopeLevel = _scopeId;
        while (scopeLevel != null)
        {
            var symbol = _symbolTable.Resolve(name, scopeLevel.Value);
            if (symbol != null)
            {
                return symbol;
            }

            scopeLevel = _scopes[scopeLevel.Value];
        }

        return null;
    }
    
    private Symbol? FindSymbol(string name, SymbolScope scope)
    {
        var symbol = _symbolTable.Symbols.FirstOrDefault(t => t.Name == name && t.SymbolScope == scope);
        if (symbol != null)
        {
            return symbol;
        }
        return null;
    }
}