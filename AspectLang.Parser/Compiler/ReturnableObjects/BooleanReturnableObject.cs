namespace AspectLang.Parser.Compiler.ReturnableObjects;

public class BooleanReturnableObject : IReturnableObject
{
    public bool Value { get; }

    public BooleanReturnableObject(bool value)
    {
        Value = value;
    }
}