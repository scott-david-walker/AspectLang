namespace AspectLang.Parser.Compiler.ReturnableObjects;

public class StringReturnableObject(string value) : IReturnableObject
{
    public string Value { get; } = value;
}