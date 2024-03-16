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
        { OpCode.Array, new ArrayOperation() },
        { OpCode.Index, new IndexOperation() },
        { OpCode.LoopBegin, new IterateOverOperation() },
        { OpCode.Increment, new IncrementOperation() },
        { OpCode.Compare, new CompareOperation() },
        { OpCode.EndLoop, new EndLoopOperation() }
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
        if (_stack.Count <= 1)
        {
            return;
        }
        _stack.Pop();
        _currentFrame = _stack.Peek();
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

internal class EndLoopOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var index = vm.Pop();
        var condition = vm.Pop();
        if (condition is not BooleanReturnableObject boolObj)
        {
            throw new ("Expected a boolean object");
        }
        if (!boolObj.Value)
        {
            vm.InstructionPointer = operands[0].Reference.Value;
        }
        else
        {
            vm.Push(index);
        }
    }
}

internal class CompareOperation : IOperation
{
    // Maybe this should just output a bool about whether to iterate again?
    public void Execute(Vm vm, List<Operand> operands)
    {
        var indexVariable = operands[0].Name;
        var compareTo = operands[1].Name;
        vm.GetLocal(indexVariable!);
        var index = vm.Pop();
        if (index is not IntegerReturnableObject indexInt)
        {
            throw new ("Expected an integer value");
        }
        vm.GetLocal(compareTo!);
        var comparable = vm.Pop();
        if (comparable is not ArrayReturnableObject arrayReturnableObject)
        {
            throw new("Expected an array value");
        }

        if (indexInt.Value < arrayReturnableObject.Elements.Count)
        {
            vm.SetLocal(arrayReturnableObject.Elements[indexInt.Value], "it");
            vm.Push(new BooleanReturnableObject(true));
        }
        else
        {
            vm.Push(new BooleanReturnableObject(false));
        }
    }
}

internal class IncrementOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var variable = operands[0];
        vm.GetLocal(variable.Name!);
        var val = vm.Pop();
        if (val is not IntegerReturnableObject intVal)
        {
            throw new ("Expected an integer value");
        }

        var value = intVal.Value;
        value++;
        vm.SetLocal(new IntegerReturnableObject(value), variable.Name!);
    }
}

internal class IterateOverOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var endOfLoop = operands[0].Reference;
        var x = vm.Pop();
        if (x is not ArrayReturnableObject array)
        {
            throw new("Expected an array object");
        }

        if (array.Elements.Count == 0)
        {
            vm.InstructionPointer = endOfLoop.Value;
        }
        else
        {
            vm.SetLocal(array.Elements[0], "it");
        }
    }
}