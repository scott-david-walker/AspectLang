using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public class ValueExpressionNode : IExpression
{
    public object Value { get; }
    public VariableType VariableType { get; }
    
    public ValueExpressionNode(object value)
    {
        Value = value;
    }
    public ValueExpressionNode(object value, VariableType variableType)
    {
        Value = value;
        VariableType = variableType;
    }

    public INode ExpressionNode()
    {
        return this;
    }
    
    public void Accept(IVisitor visitor)
    {
        
    }
}