namespace AspectLang.Parser.Compiler;

public static class ByteCode
{
    public static List<byte> Create(OpCode opcode, List<int> operands)
    {
        var definition = opcode.Find();
        if (definition.Code == OpCode.Illegal)
        {
            return Array.Empty<byte>().ToList();
        }

        var instruction = new List<byte> { (byte)opcode };
        for (var i = 0; i < operands.Count; i++)
        {
            var length = definition.Length;

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