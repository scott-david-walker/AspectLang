using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.Statements;

public class VariableDeclarationNode(Token token, bool isFreshDeclaration) : IStatement
{
    public Token Token { get; } = token;
    public string Name { get; } = token.Literal;
    public bool IsFreshDeclaration = isFreshDeclaration;

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}