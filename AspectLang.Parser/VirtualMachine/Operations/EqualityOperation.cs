using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

public class EqualityOperation : IOperation
{
    private readonly IsEqual _isEqual = new();
    public void Execute(Vm vm, List<Operand> _)
    {
        var isEqual = _isEqual.Evaluate(vm);
        
        vm.Push(isEqual ? new BooleanReturnableObject(true) : new(false));
    }
}