
using AspectLang.Parser.Ast.Statements;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.Compiler.Visitors;

public interface ILoopVisitor
{
    void Visit(IterateUntilStatement iterateUntil);
    void Visit(IterateOverStatement iterateOver);
    void Visit(BreakStatement _);
    void Visit(ContinueStatement _);
}

internal class Loop
{
    public int ConditionPointer { get; set; }
    public int EndPointer { get; set; }
    public int? InstructionToUpdate { get; set; }
}
public class LoopVisitor(Compiler compiler) : ILoopVisitor
{
    private readonly Stack<Loop> _loopStack = new();

    public void Visit(IterateUntilStatement iterateUntil)
    {
        var startPosition = compiler.Instructions.Count - 1;
        iterateUntil.Condition.Accept(compiler);
        compiler.Emit(OpCode.Compare, []);
        var pointer = compiler.Emit(OpCode.EndLoop, [new(0)]);
        Loop(iterateUntil, startPosition + 1, OpCode.Jump, new(startPosition));
        var endLoop = compiler.Instructions.Count - 1;
        compiler.UpdateInstruction(pointer, endLoop);
    }

    public void Visit(IterateOverStatement iterateOver)
    {
        compiler.Emit(OpCode.Constant, [new(compiler.AddConstant(new IntegerReturnableObject(0)))]);
        var startPosition = compiler.Instructions.Count - 1;
        compiler.Emit(OpCode.SetLocal, [new("index")]);
        compiler.Emit(OpCode.Constant, [new(compiler.AddConstant(new IntegerReturnableObject(0)))]);
        compiler.Emit(OpCode.SetLocal, [new("it")]);
        compiler.Emit(OpCode.Compare, [new("index"), new(iterateOver.Identifier.Name)]);
        
        var pointer = compiler.Emit(OpCode.EndLoop, [new(0)]); 
        Loop(iterateOver, startPosition + 1, OpCode.Increment, new("index"));
        compiler.Emit(OpCode.GetLocal, [new("index")]);
        compiler.Emit(OpCode.Jump, [new(startPosition)]);
        var endLoop = compiler.Instructions.Count - 1;
        compiler.UpdateInstruction(pointer, endLoop);    
    }

    public void Visit(BreakStatement _)
    {
        EmitAndUpdateInstruction();
    }

    public void Visit(ContinueStatement _)
    {
        EmitAndUpdateInstruction();
    }
    
    /// <summary>
    ///  Emits a jump code with a dummy reference when a continue or break is found.
    ///  Updates variable on stack with that position for the loop iterators to handle
    /// </summary>
    private void EmitAndUpdateInstruction()
    {
        var instruction = compiler.Emit(OpCode.Jump, [new(0)]);
        var loop = _loopStack.Pop();
        loop.InstructionToUpdate = instruction;
        _loopStack.Push(loop);
    }

    private void Loop(ILoop iterator, int loopStartPosition, OpCode code, Operand operand)
    {
        var loop = new Loop { ConditionPointer = loopStartPosition };
        _loopStack.Push(loop);
        iterator.Body.Accept(compiler);
        var nextInstruction = compiler.Emit(code, [operand]);
        
        // Only not null if we've encountered a break or continue
        if (loop.InstructionToUpdate != null)
        {
            loop.EndPointer = nextInstruction - 2; // exit scope as well
            compiler.UpdateInstruction(loop.InstructionToUpdate.Value, nextInstruction - 2);
        }
        _loopStack.Pop();
    }
}