using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class PrefixExpression : IExpression
{
    public string Operator { get; set; }
    public IExpression Right { get; set; }

    public PrefixExpression(string @operator)
    {
        Operator = @operator;
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