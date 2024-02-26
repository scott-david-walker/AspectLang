using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

public class NotEqualOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> _)
    {
        var isEqual = IsEqual.Evaluate(vm);
        
        vm.Push(!isEqual ? new BooleanReturnableObject(true) : new(false));
    }
}