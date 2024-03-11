using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class StringExpression(string value) : IExpression
{
    public Token Token { get; }
    public string Value { get; } = value;

    public INode ExpressionNode()
    {
        return this;
    }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}