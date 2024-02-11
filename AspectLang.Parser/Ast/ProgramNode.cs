using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public class ProgramNode : INode
{
    public List<INode> StatementNodes { get; set; } = [];
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}