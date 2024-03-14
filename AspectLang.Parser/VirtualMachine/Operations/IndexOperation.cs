using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

internal class IndexOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var codeIndex = vm.Pop();
        var lef = vm.Pop();
        ExecuteIndexExpression(lef, codeIndex, vm);
    }
    
    private void ExecuteIndexExpression(IReturnableObject lef, IReturnableObject codeIndex, Vm vm)
    {
        if (lef is ArrayReturnableObject && codeIndex is IntegerReturnableObject)
        {
            ExecuteArrayIndex(lef, codeIndex, vm);
        }
        // else if (lef.ObjectType() == HashObject.Type)
        // {
        //     ExecuteHashIndex(lef, codeIndex);
        // }
        else
        {
            throw new("Unknown index type");
        }
    }
    
    private void ExecuteArrayIndex(IReturnableObject array, IReturnableObject index, Vm vm)
    {
        var arrayObject = (ArrayReturnableObject)array;
        var max = arrayObject.Elements.Count - 1;
        var integerReturnObject = (IntegerReturnableObject)index;
        var i = integerReturnObject.Value;
        if (i < 0 || i > max) // index out of range
        {
            throw new ("Index out of range for array");
        }
        vm.Push(arrayObject.Elements.ToArray()[i]);
    }
}