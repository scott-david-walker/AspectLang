using AspectLang.Parser.Compiler;
using AspectLang.Shared;

namespace AspectLang.Parser.Ast;

public class Identifier : IExpression
{
    public Token Token { get; set; }
    public string Name { get; set; }
    public SymbolScope Scope { get; set; }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }

    public INode ExpressionNode()
    {
        return this;
    }
}