namespace AspectLang.Parser.Ast.ExpressionTypes;

public class IntegerExpression(int value) : IExpression
{
    public int Value { get; } = value;

    public INode ExpressionNode()
    {
        return this;
    }
}