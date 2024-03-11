using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public class Identifier : IExpression
{
    public string Name { get; set; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }

    public INode ExpressionNode()
    {
        return this;
    }
}