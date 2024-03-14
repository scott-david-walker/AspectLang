using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine; 
public class StackFrame(int returnLocation)
{
    private class FrameScope(FrameScope? parent)
    {
        public readonly Dictionary<string, IReturnableObject> LocalVariables = [];
        public FrameScope? Parent { get; } = parent;
    }

    public readonly int ReturnLocation = returnLocation;
    private readonly Stack<IReturnableObject> _stack = new();

    private FrameScope _currentScope = new(null);

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
        var newScope = new FrameScope(_currentScope);
        _currentScope = newScope;
    }

    public void ExitScope()
    {
        _currentScope = _currentScope.Parent;
    }

    public void GetLocal(string name)
    {
        Push(Get(name));
    }

    public void SetLocalVariable(IReturnableObject returnableObject, string name)
    {
        Set(name, returnableObject);
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
    private void Set(string name, IReturnableObject returnableObject)
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