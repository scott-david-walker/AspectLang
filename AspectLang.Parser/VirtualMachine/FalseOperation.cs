using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine;

public class FalseOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        vm.Push(new BooleanReturnableObject(false));
    }
}