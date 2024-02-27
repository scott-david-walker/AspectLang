using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine;

public static class IsEqual
{
    public static bool Evaluate(Vm vm)
    {
        var right = vm.Pop();
        var left = vm.Pop();

        return Compare(right, left);
    }

    private static bool Compare(IReturnableObject right, IReturnableObject left)
    {
        if(right is IntegerReturnableObject rightInt && left is IntegerReturnableObject leftInt)
        {
            return rightInt.Value == leftInt.Value;
        }
        
        if(right is BooleanReturnableObject rightBool && left is BooleanReturnableObject leftBool)
        {
            return rightBool.Value == leftBool.Value;
        }
        
        if(right is StringReturnableObject rightString && left is StringReturnableObject leftString)
        {
            return rightString.Value == leftString.Value;
        }

        throw new("Unable to compare two objects of different types");
    }
}