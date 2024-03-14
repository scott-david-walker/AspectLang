using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class IntegerExpression(int value, Token token) : IExpression
{
    public int Value { get; } = value;
    public Token Token { get; } = token;

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}