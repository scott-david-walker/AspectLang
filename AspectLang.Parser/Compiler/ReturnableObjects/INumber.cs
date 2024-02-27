namespace AspectLang.Parser.Compiler.ReturnableObjects;

public interface INumber : IAddable
{ 
    IReturnableObject Multiply(IReturnableObject input);
    IReturnableObject Subtract(IReturnableObject input);
    IReturnableObject Divide(IReturnableObject input);
}