using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast.ExpressionTypes;

public class FunctionCall : IExpression
{
    public Token Token { get; set; }
    public string Name { get; set; }
    public List<IExpression> Args { get; set; }
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}