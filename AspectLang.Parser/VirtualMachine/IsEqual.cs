using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine;

public static class IsEqual
{
    public static bool Evaluate(Vm vm)
    {
        var right = vm.Pop();
        var left = vm.Pop();

        var equalityChecker = (IIsEqual)left;
        return equalityChecker.IsEqual(right);
    }
}