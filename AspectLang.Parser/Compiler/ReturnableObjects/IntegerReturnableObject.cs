namespace AspectLang.Parser.Compiler.ReturnableObjects;

public class IntegerReturnableObject : IReturnableObject
{
    public int Value { get; }

    public IntegerReturnableObject(int value)
    {
        Value = value;
    }
}