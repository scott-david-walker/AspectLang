using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Ast.Statements;
using AspectLang.Parser.Compiler;
using AspectLang.Shared;

namespace AspectLang.Parser.SemanticAnalysis;

public class Analyser : IVisitor
{
    private Scope _currentScope = new(null);
    private int _scopeCount = 0;

    
    public void Analyse(INode node)
    {
        node.Accept(this);
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
        var scope = _currentScope;
        while (scope != null)
        {
            exists = scope.SymbolTable.Exists(variableAssignment.VariableDeclarationNode.Name);
            if (exists)
            {
                break;
            }

            scope = scope.Parent;
        }

        if (exists)
        {
            if (variableAssignment.VariableDeclarationNode.IsFreshDeclaration)
            {
                throw new ParserException(
                    $"Variable with name {variableAssignment.VariableDeclarationNode.Name} already exists",
                    variableAssignment.Token);
            }
            _currentScope.SymbolTable.Define(variableAssignment.VariableDeclarationNode.Name);
        }
        else
        {
            if (variableAssignment.VariableDeclarationNode.IsFreshDeclaration)
            {
                _currentScope.SymbolTable.Define(variableAssignment.VariableDeclarationNode.Name);
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
        var scope = _currentScope;
        while (scope != null)
        {
            exists = scope.SymbolTable.Exists(identifier.Name);
            if (exists)
            {
                break;
            }

            scope = scope.Parent;
        }

        if (!exists)
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
    }

    public void Visit(FunctionCall functionCall)
    {
        
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
    }

    public void Visit(IterateOverStatement iterateOver)
    {
        iterateOver.Identifier.Accept(this);
        _currentScope.SymbolTable.Define("index");
        _currentScope.SymbolTable.Define("it");
        iterateOver.Body.Accept(this);
    }

    private void EnterScope()
    {
        var scope = new Scope(_currentScope);
        _scopeCount++;
        _currentScope = scope;
    }

    private void ExitScope()
    {
        _currentScope = _currentScope.Parent!;
        _scopeCount--;
    }
}