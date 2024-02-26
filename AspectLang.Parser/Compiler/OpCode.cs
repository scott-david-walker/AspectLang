namespace AspectLang.Parser.Compiler;

public enum OpCode : byte
{
    Illegal,
    Constant,
    Sum,
    Subtract,
    Divide,
    Multiply,
    Minus,
    True,
    False,
    Equality,
    NotEqual
}

public static class DefinitionExtensions
{
    private static readonly Dictionary<byte, Definition> Opcodes = new()
    {
        { 0, new(OpCode.Illegal) },
        { 1, new(OpCode.Constant, 2) },
        { 2, new(OpCode.Sum) }
    };
        
    public static Definition Find(this OpCode opCode)
    {
        return Opcodes.TryGetValue((byte)opCode, out var value) ? value : Opcodes[0];
    }
    
    public static Definition Find(this byte opCode)
    {
        return Opcodes.TryGetValue(opCode, out var value) ? value : Opcodes[0];
    }
    
    public readonly struct Definition(OpCode code, int length)
    {
        public Definition(OpCode code) : this(code, 0)
        {
        }

        public OpCode Code { get; } = code;
        public int Length { get; } = length;
    }
}