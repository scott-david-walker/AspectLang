using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine;

public class StackFrame
{
    private readonly List<IReturnableObject> _localVariables = new();
    private readonly Stack<IReturnableObject> _stack = new();
    public void Push(IReturnableObject returnableObject)
    {
        _stack.Push(returnableObject);
    }

    public IReturnableObject Pop()
    {
        return _stack.Pop();
    }
}