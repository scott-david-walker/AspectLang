using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.Statements;

public class ReturnStatement : IStatement
{
    public Token Token { get; set; }
    public IExpression Value { get; set; }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}