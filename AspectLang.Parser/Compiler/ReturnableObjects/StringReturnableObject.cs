namespace AspectLang.Parser.Compiler.ReturnableObjects;

public class StringReturnableObject(string value) : IReturnableObject, IIsEqual, IAddable
{
    public string Value { get; } = value;
    public bool IsEqual(IReturnableObject returnableObject)
    {
        if (returnableObject is StringReturnableObject stringReturnableObject)
        {
            return Value == stringReturnableObject.Value;
        }

        throw new($"Cannot compare string with {returnableObject.GetType()}");
    }

    public IReturnableObject Add(IReturnableObject input)
    {
        if (input is StringReturnableObject stringReturnableObject)
        {
            return new StringReturnableObject(Value + stringReturnableObject.Value);
        }

        throw new($"Cannot add string with {input.GetType()}");    
    }
}