using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.VirtualMachine.Operations;

internal class ReturnOperation : IOperation
{
    public void Execute(Vm vm, List<Operand> operands)
    {
        var result = vm.Pop();
        var returnLocation = vm.ReturnLocation();
        vm.PopFrame();
        vm.Push(result);
        if (returnLocation != 0)
        {
            vm.InstructionPointer = returnLocation;
        }
    }
}