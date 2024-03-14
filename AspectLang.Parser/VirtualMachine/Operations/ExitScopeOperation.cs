using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.VirtualMachine.Operations;

internal class ExitScopeOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        vm.ExitScope();
    }
}