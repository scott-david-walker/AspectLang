using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public class VariableAssignmentNode : IStatement
{
    public VariableAssignmentNode(VariableDeclarationNode variableDeclarationNode, IExpression expression)
    {
        VariableDeclarationNode = variableDeclarationNode;
        Expression = expression;
    }
    public VariableDeclarationNode VariableDeclarationNode { get; }
    public IExpression Expression { get; }
    

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}