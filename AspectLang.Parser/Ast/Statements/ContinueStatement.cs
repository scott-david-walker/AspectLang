using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.Statements;

public class ContinueStatement : IStatement
{
    public Token Token { get; set; }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}