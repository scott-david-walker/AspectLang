using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

public class JumpWhenFalse : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var condition = vm.Pop();
        if (condition is not BooleanReturnableObject boolObj)
        {
            throw new ("Expected a boolean object");
        }
        if (!boolObj.Value)
        {
            vm.InstructionPointer = operands[0].Reference.Value;
        }
    }
}