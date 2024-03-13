using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine.Operations;

namespace AspectLang.Parser.VirtualMachine;

public class Vm
{
    private readonly List<Instruction> _instructions;
    private readonly List<IReturnableObject> _constants;
    private readonly Stack<StackFrame> _stack = new();
    private StackFrame _currentFrame = new(0);
    public int InstructionPointer { get; set; }
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
        { OpCode.Equality, new EqualityOperation() },
        { OpCode.NotEqual, new NotEqualOperation() },
        { OpCode.LessThan, new LessThanOperation() },
        { OpCode.LessThanEqualTo, new LessThanEqualToOperation() },
        { OpCode.GreaterThan, new GreaterThanOperation() },
        { OpCode.GreaterThanEqualTo, new GreaterThanEqualToOperation() },
        { OpCode.Negate, new NegateOperation() },
        { OpCode.JumpWhenFalse, new JumpWhenFalse() },
        { OpCode.Jump, new JumpOperation() },
        { OpCode.Return, new ReturnOperation() },
        { OpCode.EnterScope, new EnterScopeOperation() },
        { OpCode.ExitScope, new ExitScopeOperation() },
        { OpCode.SetLocal, new SetLocalOperation() },
        { OpCode.SetLocalArgument, new SetLocalArgumentOperation() },
        { OpCode.GetLocal, new GetLocalOperation() },
        { OpCode.JumpToFunction, new JumpToFunctionOperation() },
    };
    public Vm(List<Instruction> instructions, List<IReturnableObject> constants)
    {
        _instructions = instructions;
        _constants = constants;
        _stack.Push(_currentFrame);
    }

    public IReturnableObject Run()
    {
        for (InstructionPointer = 0; InstructionPointer < _instructions.Count; InstructionPointer++)
        {
            var instruction = _instructions[InstructionPointer];
            if (instruction.OpCode == OpCode.Halt)
            {
                break;
            }

            if (instruction.OpCode == OpCode.SetLocal)
            {
                
            }
            if (_operations.TryGetValue(instruction.OpCode, out var op))
            {
                op.Execute(this, instruction.Operands);
            }
        }

        return _currentFrame.Pop();
    }

    public void Push(IReturnableObject returnableObject)
    {
        _currentFrame.Push(returnableObject);
    }

    public IReturnableObject Pop()
    {
        return _currentFrame.Pop();
    }

    public void PushFrame(int returnLocation)
    {
        var frame = new StackFrame(returnLocation);
        _currentFrame = frame;
        _stack.Push(frame);
    }

    public void PopFrame()
    {
        if (_stack.Count > 1)
        {
            _stack.Pop();
            _currentFrame = _stack.Peek();
        }
    }

    public int ReturnLocation()
    {
        return _currentFrame.ReturnLocation;
    }
    
    public IReturnableObject GetConstant(int index)
    {
        return _constants[index];
    }
    public void SetLocal(IReturnableObject returnableObject, string name)
    {
        _currentFrame.SetLocalVariable(returnableObject, name);
    }
    
    public void GetLocal(string name)
    {
        _currentFrame.GetLocal(name);
    }

    public void EnterScope()
    {
        _currentFrame.EnterScope();
    }
    public void ExitScope()
    {
        _currentFrame.ExitScope();
    }
}

internal class JumpToFunctionOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        vm.InstructionPointer = operands[0].Reference.Value;
        var returnLocation = operands[1].Reference.Value;
        var argCount = operands[2].Reference.Value;
        var x = new List<IReturnableObject>();
        for (int i = 0; i < argCount; i++)
        {
            x.Add(vm.Pop());
        }
        vm.PushFrame(returnLocation);
        //x.Reverse();
        foreach (var y in x)
        {
            vm.Push(y);
        }
    }
}

internal class GetLocalOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var variableName = operands[0].Name;
        vm.GetLocal(variableName);
    }
}

internal class SetLocalOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var variableName = operands[0].Name;
        var expression = vm.Pop();
        vm.SetLocal(expression, variableName);
    }
}

internal class EnterScopeOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        vm.EnterScope();
    }
}

internal class ExitScopeOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        vm.ExitScope();
    }
}
internal class ReturnOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var result = vm.Pop();
        var returnLocation = vm.ReturnLocation();
        vm.PopFrame();
        vm.Push(result);
        if (returnLocation != 0)
        {
            vm.InstructionPointer = returnLocation;
        }
    }
}