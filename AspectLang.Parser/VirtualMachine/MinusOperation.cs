using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine;

public class MinusOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var returnableObject = vm.Pop();
        if (returnableObject is IntegerReturnableObject integerReturnableObject)
        {
            vm.Push(new IntegerReturnableObject(-integerReturnableObject.Value));
        }
        else
        {
            throw new("Expected an integer object");
        }
    }
}