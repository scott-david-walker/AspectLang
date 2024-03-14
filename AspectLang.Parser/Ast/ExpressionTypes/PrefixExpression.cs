using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class PrefixExpression : IExpression
{
    public Token Token { get;  }
    public string Operator => Token.Literal;
    public IExpression Right { get; set; }

    public PrefixExpression(Token token)
    {
        Token = token;
    }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}