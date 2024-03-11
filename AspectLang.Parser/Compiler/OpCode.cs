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
    NotEqual,
    Negate,
    JumpWhenFalse,
    Jump,
    SetGlobal,
    GetGlobal,
    Return,
    EnterScope,
    ExitScope,
    SetLocal,
    GetLocal
}

public static class DefinitionExtensions
{
    private static readonly Dictionary<OpCode, int> Opcodes = new()
    {
        { OpCode.Constant, 2},
        { OpCode.JumpWhenFalse, 2},
        { OpCode.Jump, 2},
        { OpCode.GetGlobal, 2},
        { OpCode.SetGlobal, 2},
        { OpCode.GetLocal, 2},
        { OpCode.SetLocal, 2},
    };

    public static int FindLength(this OpCode opCode)
    {
        return Opcodes.ContainsKey(opCode) ? Opcodes[opCode] : 0;
    }
}