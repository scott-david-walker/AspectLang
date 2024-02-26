using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine;

public class IsEqual
{
    public bool Evaluate(Vm vm)
    {
        var right = vm.Pop();
        var left = vm.Pop();

        // Will change when we want to evaluate other objects
        var rightInt = (IntegerReturnableObject)right;
        var leftInt = (IntegerReturnableObject)left;

        return rightInt.Value == leftInt.Value;
    }
}