using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.Statements;

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
    public Token Token { get; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}