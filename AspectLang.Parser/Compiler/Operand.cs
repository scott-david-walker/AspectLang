namespace AspectLang.Parser.Compiler;

public class Operand
{
    public Operand(int reference)
    {
        OperandType = OperandType.Pointer;
        Reference = reference;
    }
    
    public Operand(string name)
    {
        OperandType = OperandType.Name;
        Name = name;
    }
    public OperandType OperandType { get; set; }
    public int? Reference { get; set; }
    public string? Name { get; set; }
}