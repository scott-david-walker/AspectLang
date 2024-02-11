using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.VirtualMachine;

public interface IOperation
{
    void Execute(Vm vm, List<Operand> operands);
}