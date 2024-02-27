namespace AspectLang.Parser.Compiler.ReturnableObjects;

public class BooleanReturnableObject : IReturnableObject, IIsEqual
{
    public bool Value { get; }

    public BooleanReturnableObject(bool value)
    {
        Value = value;
    }

    public bool IsEqual(IReturnableObject returnableObject)
    {
        if (returnableObject is BooleanReturnableObject booleanReturnableObject)
        {
            return Value == booleanReturnableObject.Value;
        }

        throw new($"Cannot compare boolean with {returnableObject.GetType()}");
    }
}