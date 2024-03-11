using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Shared;

namespace AspectLang.Parser.VirtualMachine;

public class StackFrame
{
    private readonly List<IReturnableObject> _localVariables = new();
    private readonly Stack<IReturnableObject> _stack = new();
    private Scope? _currentScope;
    public void Push(IReturnableObject returnableObject)
    {
        _stack.Push(returnableObject);
    }

    public IReturnableObject Pop()
    {
        return _stack.Pop();
    }

    public void EnterScope()
    {
        if (_currentScope == null)
        {
            _currentScope = new(null);
        }
        else
        {
            var newScope = new Scope(_currentScope);
            newScope.Parent = _currentScope;
            _currentScope = newScope;
        }
    }

    public void ExitScope()
    {
        _currentScope = _currentScope!.Parent;
    }
}