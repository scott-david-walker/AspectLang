namespace AspectLang.Parser.Compiler;

public enum OpCode : byte
{
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
    Return,
    EnterScope,
    ExitScope,
    SetLocal,
    GetLocal,
    JumpToFunction,
    Halt,
    SetLocalArgument,
    LessThan,
    LessThanEqualTo,
    GreaterThan,
    GreaterThanEqualTo,
    Array,
    Index
}