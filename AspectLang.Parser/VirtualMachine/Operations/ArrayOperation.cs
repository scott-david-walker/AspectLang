using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

internal class ArrayOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var numberOfElements = operands[0].Reference;
        var array = BuildArray(vm, numberOfElements.Value);
        vm.Push(array);
    }
    
    private IReturnableObject BuildArray(Vm vm, int count)
    {
        var elements = new List<IReturnableObject>();
        for (var i = 0; i < count; i++)
        {
            elements.Add(vm.Pop());
        }

        elements.Reverse();
        return new ArrayReturnableObject
        {
            Elements = elements
        };
    }
}