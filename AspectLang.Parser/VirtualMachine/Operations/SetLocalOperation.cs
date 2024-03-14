using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.VirtualMachine.Operations;

internal class SetLocalOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var variableName = operands[0].Name;
        var expression = vm.Pop();
        vm.SetLocal(expression, variableName);
    }
}