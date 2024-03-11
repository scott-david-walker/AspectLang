using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public class ReturnStatement : IStatement
{
    public Token Token { get; }
    public IExpression Value { get; set; }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}