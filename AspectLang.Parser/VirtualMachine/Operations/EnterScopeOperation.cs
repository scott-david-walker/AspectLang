using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.VirtualMachine.Operations;

internal class EnterScopeOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        vm.EnterScope();
    }
}