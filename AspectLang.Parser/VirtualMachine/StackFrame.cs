using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Shared;

namespace AspectLang.Parser.VirtualMachine;


public class StackFrame(int returnLocation)
{
    private class TestScope(TestScope? parent)
    {
        public readonly Dictionary<string, IReturnableObject> LocalVariables = [];
        public TestScope? Parent { get; set; } = parent;
    }

    public readonly int ReturnLocation = returnLocation;
    private readonly Stack<IReturnableObject> _stack = new();

    private TestScope _currentScope = new(null);

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
            var newScope = new TestScope(_currentScope);
            _currentScope = newScope;
        }
    }

    public void ExitScope()
    {
        _currentScope = _currentScope!.Parent;
    }

    public void GetLocal(int localLocation, string name)
    {
        Push(Get(name));
    }

    public void SetLocalVariable(IReturnableObject returnableObject, int location, string name)
    {
        SetOrThrow(name, returnableObject);
    }

    private IReturnableObject Get(string name)
    {
        var s = _currentScope;
        while (s != null)
        {
            if(s.LocalVariables.ContainsKey(name))
            {
                return s.LocalVariables[name];
            }

            s = s.Parent;
        }

        throw new("No variable found");
    }
    private void SetOrThrow(string name, IReturnableObject returnableObject)
    {
        var s = _currentScope;
        while (s != null)
        {
            if(s.LocalVariables.ContainsKey(name))
            {
                s.LocalVariables[name] = returnableObject;
                return;
            }

            s = s.Parent;
        }

        _currentScope.LocalVariables[name] = returnableObject;
    }
}