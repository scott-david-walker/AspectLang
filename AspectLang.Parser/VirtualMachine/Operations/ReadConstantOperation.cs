using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.VirtualMachine.Operations;

public class ReadConstantOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var constantIndex = operands[0].Reference.Value; // expecting only one here
        vm.Push(vm.GetConstant(constantIndex));
    }
}