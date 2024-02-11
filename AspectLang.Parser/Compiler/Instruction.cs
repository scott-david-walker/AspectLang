namespace AspectLang.Parser.Compiler;

public class Instruction
{
    public OpCode OpCode { get; set; }
    public List<Operand> Operands { get; set; } = new();

    public Instruction()
    {
        
    }
    public Instruction(OpCode opCode)
    {
        OpCode = opCode;
    }
}