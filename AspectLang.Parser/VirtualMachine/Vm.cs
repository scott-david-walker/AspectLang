using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine;

public class Vm
{
    private readonly List<Instruction> _instructions;
    private readonly List<IReturnableObject> _constants;
    private readonly Stack<IReturnableObject> _stack = new();

    private readonly Dictionary<OpCode, IOperation> _operations = new()
    {
        { OpCode.Constant, new ReadConstantOperation() },
        { OpCode.Sum, new AddOperation() },
        { OpCode.Subtract, new SubtractOperation() },
        { OpCode.Divide, new DivideOperation() },
        { OpCode.Multiply, new MultiplyOperation() },
        { OpCode.Minus, new MinusOperation() },
        { OpCode.True, new TrueOperation() },
        { OpCode.False, new FalseOperation() },
        { OpCode.Equality, new EqualityOperation() }
    };
    public Vm(List<Instruction> instructions, List<IReturnableObject> constants)
    {
        _instructions = instructions;
        _constants = constants;
    }

    public IReturnableObject Run()
    {
        foreach (var instruction in _instructions)
        {
            if (_operations.TryGetValue(instruction.OpCode, out var op))
            {
                op.Execute(this, instruction.Operands);
            }
        }

        return _stack.Pop();
    }

    public void Push(IReturnableObject returnableObject)
    {
        _stack.Push(returnableObject);
    }

    public IReturnableObject Pop()
    {
        return _stack.Pop();
    }

    public IReturnableObject GetConstant(int index)
    {
        return _constants[index];
    }
}