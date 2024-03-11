using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
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
        var scope = new Scope(_currentScope);
        _scopeCount++;
        _currentScope = scope;
        foreach (var statement in blockStatement.Statements)
        {
            statement.Accept(this);
        }

        _currentScope = scope.Parent!;
        _scopeCount--;
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
        var symbolScope = _scopeCount == 0 ? SymbolScope.Global : SymbolScope.Local;
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
            variableAssignment.Scope = symbolScope;
            _currentScope.SymbolTable.Define(variableAssignment.VariableDeclarationNode.Name, symbolScope);
        }
        else
        {
            if (variableAssignment.VariableDeclarationNode.IsFreshDeclaration)
            {
                variableAssignment.Scope = symbolScope;
                _currentScope.SymbolTable.Define(variableAssignment.VariableDeclarationNode.Name, symbolScope);
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
        var symbolScope = _scopeCount == 0 ? SymbolScope.Global : SymbolScope.Local;
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

        if (exists)
        {
            identifier.Scope = symbolScope;
        }
        else
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
}