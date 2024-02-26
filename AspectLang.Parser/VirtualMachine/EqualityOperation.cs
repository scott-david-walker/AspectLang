using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine;

public class EqualityOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var right = vm.Pop();
        var left = vm.Pop();

        var rightInt = (IntegerReturnableObject)right;
        var leftInt = (IntegerReturnableObject)left;

        if (rightInt.Value == leftInt.Value)
        {
            vm.Push(new BooleanReturnableObject(true));
        }
        else
        {
            vm.Push(new BooleanReturnableObject(false));
        }
    }
}