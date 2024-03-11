using AspectLang.Parser.Compiler;
using AspectLang.Shared;

namespace AspectLang.Parser.Ast;

public class VariableAssignmentNode : IStatement
{
    public VariableAssignmentNode(VariableDeclarationNode variableDeclarationNode, IExpression expression, Token token)
    {
        Token = token;
        VariableDeclarationNode = variableDeclarationNode;
        Expression = expression;
    }
    public VariableDeclarationNode VariableDeclarationNode { get; }
    public IExpression Expression { get; }
    public SymbolScope Scope { get; set; }
    public Token Token { get; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}