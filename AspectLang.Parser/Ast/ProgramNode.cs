namespace AspectLang.Parser.Ast;

public class ProgramNode : INode
{
    public List<INode> StatementNodes { get; set; } = [];
}