using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.Statements;

public class BlockStatement : IStatement
{
    public List<IStatement> Statements { get; set; } = [];
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}