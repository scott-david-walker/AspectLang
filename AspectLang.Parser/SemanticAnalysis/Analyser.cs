using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Ast.Statements;
using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.SemanticAnalysis;

public enum TestScope
{
    Function,
    Local
}

public class TestSymbol
{
    public string Name { get; set; }
    public TestScope TestScope { get; set; }
    public int ScopeLevel { get; set; }
    public Guid ScopeId { get; set; }
    public Guid? ParentScopeId { get; set; }
}
public class TestSymbolTable
{
    public List<TestSymbol> Symbols { get; set; } = [];
}

public class Analysis
{
    public Dictionary<Guid, Guid?> Scopes { get; set; }
    public TestSymbolTable TestSymbolTable { get; set; }
    public List<Guid> EnterScopes { get; set; }
}
public class AnalyserResult
{
    public Analysis Analysis { get; set; }
    public ParserError Error { get; set; }
    
    public ProgramNode ProgramNode { get; } = new();
    public List<ParserError> Errors { get; } = [];

    public AnalyserResult()
    {
        
    }
    public AnalyserResult(Analysis analysis)
    {
        Analysis = analysis;
    }
    
    public AnalyserResult(ParserError parserError)
    {
        Errors.Add(parserError);
    }
}
public class Analyser : IVisitor
{
    private TestSymbolTable _testSymbolTable = new();
    private int _scopeCount;
    private Guid _scopeId = Guid.Parse("2d54b924-5671-408a-8e3d-8d7a25b2043a");
    private List<Guid> _enterScopes = [];
    private Dictionary<Guid, Guid?> _scopes { get; set; } = [];
    private List<FunctionCall> _functionCalls = [];
    private List<FunctionDeclarationStatement> _functions = [];
    public AnalyserResult Analyse(INode node)
    {
        try
        {
            _scopes.Add(_scopeId, null);
            node.Accept(this);
            AnalyseFunctionDeclarations();
            AnalyseFunctionCalls();
            var res = new Analysis()
            {
                TestSymbolTable = _testSymbolTable,
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
            _testSymbolTable.Symbols.Add(new()
            {
                Name = functionDeclaration.Name,
                ScopeLevel = _scopeCount,
                TestScope = TestScope.Function,
                ScopeId = _scopeId,
                ParentScopeId = null
            });
            foreach (var parameters in functionDeclaration.Parameters)
            {
                _testSymbolTable.Symbols.Add(new()
                {
                    Name = parameters.Name,
                    ScopeLevel = _scopeCount,
                    TestScope = TestScope.Local,
                    ScopeId = _scopeId,
                    ParentScopeId = null
                });
                parameters.Accept(this);
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
           var symbol = FindSymbol(functionCall.Name, TestScope.Function);
           if (symbol == null)
           {
               throw new ("Function not found or something");
           }

           //var func = _functions.FirstOrDefault(t => t.Name == functionCall.Name);
           
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
        var exists = false;
        var symbol = FindSymbol(variableAssignment.VariableDeclarationNode.Name);
        if (symbol != null)
        {
            exists = true;
        }
        if (exists)
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
                _testSymbolTable.Symbols.Add(new()
                {
                    Name = variableAssignment.VariableDeclarationNode.Name,
                    ScopeLevel = _scopeCount,
                    TestScope = TestScope.Local,
                    ScopeId = _scopeId,
                    ParentScopeId = _scopes.ContainsKey(_scopeId) ? _scopes[_scopeId] : null
                });
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
        var exists = false;
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
        _testSymbolTable.Symbols.Add(new()
        {
            Name = "it",
            ScopeLevel = _scopeCount,
            TestScope = TestScope.Local,
            ScopeId = _scopeId,
            ParentScopeId = _scopes.ContainsKey(_scopeId) ? _scopes[_scopeId] : null
        });
        _testSymbolTable.Symbols.Add(new()
        {
            Name = "index",
            ScopeLevel = _scopeCount,
            TestScope = TestScope.Local,
            ScopeId = _scopeId,
            ParentScopeId = _scopes.ContainsKey(_scopeId) ? _scopes[_scopeId] : null
        });
        iterateOver.Body.Accept(this);
    }

    private void EnterScope()
    {
        var scopeId = Guid.NewGuid();
        _enterScopes.Add(scopeId);
        _scopes.Add(scopeId, _scopeId);
        _scopeId = scopeId;
        _scopeCount++;
    }

    private void ExitScope()
    {
        var scope = _scopes[_scopeId];
        _scopeId = scope.Value;
        _scopeCount--;
    }

    private TestSymbol? FindSymbol(string name)
    {
        Guid? scopeLevel = _scopeId;
        while (scopeLevel != null)
        {
            var symbol = _testSymbolTable.Symbols.FirstOrDefault(t => t.Name == name && t.ScopeId == scopeLevel);
            if (symbol != null)
            {
                return symbol;
            }

            scopeLevel = _scopes[scopeLevel.Value];
        }

        return null;
    }
    
    private TestSymbol? FindSymbol(string name, TestScope scope)
    {
        var symbol = _testSymbolTable.Symbols.FirstOrDefault(t => t.Name == name && t.TestScope == scope);
        if (symbol != null)
        {
            return symbol;
        }
        return null;
    }
}