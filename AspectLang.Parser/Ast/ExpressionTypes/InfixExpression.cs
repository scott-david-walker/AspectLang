using System.Text;
using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class InfixExpression : IExpression
{
    public string Operator { get; set; }
    public IExpression Right { get; set; }
    public IExpression Left { get; set; }
    
    public INode ExpressionNode()
    {
        return this;
    }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}