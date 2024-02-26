using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.VirtualMachine.Operations;

public interface IOperation
{
    void Execute(Vm vm, List<Operand> operands);
}