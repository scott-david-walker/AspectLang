namespace AspectLang.Parser.Compiler;

public static class ByteCode
{
    public static List<byte> Create(OpCode opcode, List<int> operands)
    {
        var length = opcode.FindLength();
        var instruction = new List<byte> { (byte)opcode };
        for (var i = 0; i < operands.Count; i++)
        {
            switch (length)
            {
                case 1:
                    instruction.Add((byte)operands[i]);
                    break;
                case 2:
                    instruction.AddRange(BitConverter.GetBytes(Convert.ToUInt16(operands[i])));
                    break;
            }
        }

        return instruction;
    }
}