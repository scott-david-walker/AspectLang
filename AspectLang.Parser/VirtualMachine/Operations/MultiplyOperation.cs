using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

public class MultiplyOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var right = vm.Pop();
        var left = vm.Pop();
        var leftNumber = (INumber)left;
        var ret = leftNumber.Multiply(right);
        vm.Push(ret);
    }
    
}