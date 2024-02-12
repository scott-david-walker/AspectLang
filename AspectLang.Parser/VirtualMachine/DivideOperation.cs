using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine;


public class DivideOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var right = vm.Pop();
        var left = vm.Pop();

        var rightInt = (IntegerReturnableObject)right;
        var leftInt = (IntegerReturnableObject)left;

        var ret = new IntegerReturnableObject(leftInt.Value / rightInt.Value);
                    
        vm.Push(ret);
    }
}