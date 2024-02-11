namespace AspectLang.Parser.Compiler;

public class IntegerReturnableObject : IReturnableObject
{
    public int Value { get; }

    public IntegerReturnableObject(int value)
    {
        Value = value;
    }
}