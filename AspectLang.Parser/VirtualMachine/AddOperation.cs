using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.VirtualMachine;

public class AddOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var right = vm.Pop();
        var left = vm.Pop();

        var rightInt = (IntegerReturnableObject)right;
        var leftInt = (IntegerReturnableObject)left;

        var ret = new IntegerReturnableObject(rightInt.Value + leftInt.Value);
                    
        vm.Push(ret);
    }
}