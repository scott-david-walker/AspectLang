namespace AspectLang.Parser.Ast;

public class VariableAssignmentNode : INode
{
    public VariableAssignmentNode(VariableDeclarationNode variableDeclarationNode, IExpression expression)
    {
        VariableDeclarationNode = variableDeclarationNode;
        Expression = expression;
    }
    public VariableDeclarationNode VariableDeclarationNode { get; }
    public IExpression Expression { get; }

    public string AsStringValue()
    {
        return "";
    }
}