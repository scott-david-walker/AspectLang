using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class BooleanExpression : IExpression
{
    public bool Value { get; set; }

    public BooleanExpression(bool value)
    {
        Value = value;
    }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }

    public INode ExpressionNode()
    {
        return this;
    }
}