namespace AspectLang.Parser.Ast;

public class ExpressionStatement : IStatement
{
    public IExpression Expression { get; set; }
}