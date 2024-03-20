using AspectLang.Parser.Ast.Statements;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.Compiler.Visitors;

public interface ILoopVisitor
{
    void Visit(IterateUntilStatement iterateUntil, Compiler compiler);
    void Visit(IterateOverStatement iterateUntil, Compiler compiler);

    void PushLoop(Loop loop);
    Loop PopLoop();
}

public class LoopVisitor : ILoopVisitor
{
    private Stack<Loop> _loopStack = new();

    public void Visit(IterateUntilStatement iterateUntil, Compiler compiler)
    {
        var startPosition = compiler.Instructions.Count - 1;
        iterateUntil.Condition.Accept(compiler);
        compiler.Emit(OpCode.Compare, []);
        var pointer = compiler.Emit(OpCode.EndLoop, [new(0)]);
        var loop = new Loop { ConditionPointer = startPosition + 1 };
        _loopStack.Push(loop);
        iterateUntil.Body.Accept(compiler);
        var jumpPointer = compiler.Emit(OpCode.Jump, [new(startPosition)]);
        if (loop.InstructionToUpdate != null)
        {
            loop.EndPointer = jumpPointer - 2; // exit scope as well
            compiler.UpdateInstruction(loop.InstructionToUpdate.Value, jumpPointer - 2);
        }

        _loopStack.Pop();

        var endLoop = compiler.Instructions.Count - 1;
        compiler.UpdateInstruction(pointer, endLoop);
    }

    public void Visit(IterateOverStatement iterateOver, Compiler compiler)
    {
        compiler.Emit(OpCode.Constant, [new(compiler.AddConstant(new IntegerReturnableObject(0)))]);
        var startPosition = compiler.Instructions.Count - 1;
        compiler.Emit(OpCode.SetLocal, [new("index")]);
        compiler.Emit(OpCode.Constant, [new(compiler.AddConstant(new IntegerReturnableObject(0)))]);
        compiler.Emit(OpCode.SetLocal, [new("it")]);
        compiler.Emit(OpCode.Compare, [new("index"), new(iterateOver.Identifier.Name)]);
        
        var pointer = compiler.Emit(OpCode.EndLoop, [new(0)]); 
        var loop = new Loop { ConditionPointer = startPosition + 1 };
        _loopStack.Push(loop);
        iterateOver.Body.Accept(compiler);
        var incrementPointer = compiler.Emit(OpCode.Increment, [new("index")]);
        if (loop.InstructionToUpdate != null)
        {
            loop.EndPointer = incrementPointer - 2; // exit scope as well
            compiler.UpdateInstruction(loop.InstructionToUpdate.Value, incrementPointer - 2);
        }
        _loopStack.Pop();
        compiler.Emit(OpCode.GetLocal, [new("index")]);
        compiler.Emit(OpCode.Jump, [new(startPosition)]);
        var endLoop = compiler.Instructions.Count - 1;
        compiler.UpdateInstruction(pointer, endLoop);    }

    public void PushLoop(Loop loop)
    {
        _loopStack.Push(loop);
    }

    public Loop PopLoop()
    {
        return _loopStack.Pop();
    }
}