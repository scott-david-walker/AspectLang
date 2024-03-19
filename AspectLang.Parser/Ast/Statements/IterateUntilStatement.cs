using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.Statements;

public class IterateUntilStatement : IStatement
{
    public Token Token { get; set; }
    public BlockStatement Body { get; set; }
    public IExpression Condition { get; set; }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}