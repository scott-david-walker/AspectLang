using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.VirtualMachine;

public class Vm
{
    private readonly List<byte> _instructions;
    private readonly List<IReturnableObject> _constants;
    private readonly Stack<IReturnableObject> _stack = new();
    public Vm(List<byte> instructions, List<IReturnableObject> constants)
    {
        _instructions = instructions;
        _constants = constants;
    }

    public IReturnableObject Run()
    {
        for (var i = 0; i < _instructions.Count; i++)
        {
            var opcode = _instructions[i].Find();

            switch (opcode.Code)
            {
                case OpCode.Constant:
                    var constantIndex = BitConverter.ToInt16(_instructions.ToArray(), startIndex: i + 1);
                    i += 2; // constants are two bytes wide
                    PushToStack(_constants[constantIndex]);
                    break;
                
                case OpCode.Sum:
                    var right = _stack.Pop();
                    var left = _stack.Pop();

                    var rightInt = (IntegerReturnableObject)right;
                    var leftInt = (IntegerReturnableObject)left;

                    var ret = new IntegerReturnableObject(rightInt.Value + leftInt.Value);
                    
                    PushToStack(ret);
                    break;
            }
        }

        var p = _stack.Pop() as IntegerReturnableObject;
        Console.WriteLine(p.Value);
        return null;
    }

    private void PushToStack(IReturnableObject returnableObject)
    {
        _stack.Push(returnableObject);
    }
}