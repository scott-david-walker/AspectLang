using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

public class AddOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var right = vm.Pop();
        var left = vm.Pop();
        var rightAddable = (IAddable)right;
        var ret = rightAddable.Add(left);
        vm.Push(ret);
    }
}