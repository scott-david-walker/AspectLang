namespace AspectLang.Parser.Ast;

public class VariableDeclarationNode : INode
{
    public string Name { get; }
    public VariableDeclarationNode(Token token)
    {
        Name = token.Literal;
    }

    public string AsStringValue()
    {
        return Name;
    }
}