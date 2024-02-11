using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public class VariableDeclarationNode : INode
{
    public string Name { get; }
    public VariableDeclarationNode(Token token)
    {
        Name = token.Literal;
    }
    
    public void Accept(IVisitor visitor)
    {
        
    }
}