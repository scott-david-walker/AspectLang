using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

internal class GreaterThanEqualToOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var right = vm.Pop();
        var left = vm.Pop();
        var leftInt = left as IntegerReturnableObject;
        var rightInt = right as IntegerReturnableObject;
        vm.Push(leftInt.Value >= rightInt.Value ? new BooleanReturnableObject(true) : new(false));
    }
}