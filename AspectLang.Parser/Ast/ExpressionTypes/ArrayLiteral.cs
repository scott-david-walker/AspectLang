using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class ArrayLiteral : IExpression
{
    public Token Token { get; set; }
    public List<IExpression> Elements { get; set; } = new();

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}