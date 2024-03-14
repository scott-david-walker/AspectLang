using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.Statements;

public class FunctionDeclarationStatement : IStatement
{
    public Token Token { get; set; }
    public string Name { get; set; }
    public List<Identifier> Parameters = [];
    public BlockStatement Body { get; set; }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}