using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public class VariableDeclarationNode(Token token) : INode
{
    public string Name { get; } = token.Literal;

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}