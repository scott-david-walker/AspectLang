using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.VirtualMachine.Operations;

internal class GetLocalOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var variableName = operands[0].Name;
        vm.GetLocal(variableName);
    }
}