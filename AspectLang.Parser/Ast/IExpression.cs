namespace AspectLang.Parser.Ast;

public interface IExpression : INode
{
    INode ExpressionNode();
}