using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public class IfStatement : IStatement
{
    public IExpression Condition { get; set; }
    public BlockStatement Consequence { get; set; }
    public BlockStatement? Alternative { get; set; }
    
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}