using System.Text;
using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class InfixExpression : IExpression
{
    public Token Token { get; set; }
    public string Operator { get; set; }
    public IExpression Right { get; set; }
    public IExpression Left { get; set; }
    
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}