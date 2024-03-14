using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class BooleanExpression : IExpression
{
    public bool Value { get; set; }
    public Token Token { get; }

    public BooleanExpression(bool value, Token token)
    {
        Value = value;
        Token = token;
    }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}