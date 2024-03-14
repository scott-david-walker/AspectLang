using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.VirtualMachine.Operations;

internal class JumpToFunctionOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        vm.InstructionPointer = operands[0].Reference.Value;
        var returnLocation = operands[1].Reference.Value;
        var argCount = operands[2].Reference.Value;
        var x = new List<IReturnableObject>();
        for (var i = 0; i < argCount; i++)
        {
            x.Add(vm.Pop());
        }
        vm.PushFrame(returnLocation);
        foreach (var y in x)
        {
            vm.Push(y);
        }
    }
}