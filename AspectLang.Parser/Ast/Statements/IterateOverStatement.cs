using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.Statements;

public class IterateOverStatement : IStatement, ILoop
{
    public Token Token { get; set; }
    public Identifier Identifier { get; set; }
    public BlockStatement Body { get; set; }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}