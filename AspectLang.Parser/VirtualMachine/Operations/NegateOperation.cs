using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

public class NegateOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var expectedBoolean = vm.Pop();
        if (expectedBoolean is not BooleanReturnableObject obj)
        {
            throw new("Expected Boolean in negate");
        }

        vm.Push(obj.Value ? new BooleanReturnableObject(false) : new (true));
    }
}