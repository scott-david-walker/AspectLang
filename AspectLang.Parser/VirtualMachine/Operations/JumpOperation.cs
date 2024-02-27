using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

public class JumpOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        vm.InstructionPointer = operands[0].Reference.Value;
    }
}