using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class IndexExpression : IExpression
{
    public Token Token { get; set; }
    public IExpression Left { get; set; }
    public IExpression Index { get; set; }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }

    public INode ExpressionNode()
    {
        return this;
    }
}