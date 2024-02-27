namespace AspectLang.Parser.Compiler.ReturnableObjects;

public class IntegerReturnableObject : IReturnableObject, INumber, IIsEqual
{
    public int Value { get; }

    public IntegerReturnableObject(int value)
    {
        Value = value;
    }

    public IReturnableObject Add(IReturnableObject input)
    {
        if (input is IntegerReturnableObject integerReturnableObject)
        {
            return new IntegerReturnableObject(Value + integerReturnableObject.Value);
        }

        throw new($"Cannot add integer to {input.GetType()}");
    }

    public IReturnableObject Multiply(IReturnableObject input)
    {
        if (input is IntegerReturnableObject integerReturnableObject)
        {
            return new IntegerReturnableObject(Value * integerReturnableObject.Value);
        }

        throw new($"Cannot Multiple integer with {input.GetType()}");
    }

    public IReturnableObject Subtract(IReturnableObject input)
    {
        if (input is IntegerReturnableObject integerReturnableObject)
        {
            return new IntegerReturnableObject(Value - integerReturnableObject.Value);
        }

        throw new($"Cannot Subtract integer with {input.GetType()}");
    }

    public IReturnableObject Divide(IReturnableObject input)
    {
        if (input is IntegerReturnableObject integerReturnableObject)
        {
            return new IntegerReturnableObject(Value / integerReturnableObject.Value);
        }

        throw new($"Cannot Divide integer with {input.GetType()}");
    }

    public bool IsEqual(IReturnableObject returnableObject)
    {
        if (returnableObject is IntegerReturnableObject integerReturnableObject)
        {
            return Value == integerReturnableObject.Value;
        }

        throw new($"Cannot Compare integer with {returnableObject.GetType()}");    
    }
}