using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public class ExpressionStatement : IStatement
{
    public IExpression Expression { get; set; }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}