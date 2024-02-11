using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Compiler.ReturnableObjects;

namespace AspectLang.Parser.Compiler;

public class Compiler : IVisitor
{
    public List<Instruction> Instructions { get; } = [];
    public List<IReturnableObject> Constants { get; } = [];
    public void Compile(INode node)
    {
        node.Accept(this);
        
        // Should do a conversion to some byte code format tbd
    }
    
    public void Visit(IntegerExpression expression)
    {
        var val = expression.Value;
        Emit(OpCode.Constant, [AddConstant(new IntegerReturnableObject(val))]);
    }

    public void Visit(ProgramNode programNode)
    {
        foreach (var statement in programNode.StatementNodes)
        {
            statement.Accept(this);
        }
    }

    public void Visit(ExpressionStatement expression)
    {
        expression.Expression.Accept(this);
    }

    public void Visit(InfixExpression expression)
    {
        expression.Left.Accept(this);
        expression.Right.Accept(this);
        switch (expression.Operator)
        {
            case "+":
                Emit(OpCode.Sum);
                break;
        }
    }
    
    private void Emit(OpCode opcode)
    {
        Emit(opcode, []);
    }
    
    private int Emit(OpCode opcode, List<int> operands)
    {
        //var instructions = ByteCode.Create(opcode, operands);
        var instruction = CreateInstruction(opcode, operands);
        var position = AddInstructions(instruction);
        //SetLastInstruction((byte)opcode, position);
        return position;
    }

    private static Instruction CreateInstruction(OpCode opCode, List<int> operands)
    {
        if (!operands.Any())
        {
            return new() { OpCode = opCode };
        }

        if (opCode == OpCode.Constant)
        {
            if (operands.Count > 1)
            {
                // We only ever expect a constant to point to an index. In reality, the length should only be one..
                throw new("Expected opcode to contain index of constant. Length too long");
            }
            var operand = new Operand
            {
                OperandType = OperandType.Pointer,
                Reference = operands[0]
            };
            return new()
            {
                OpCode = opCode,
                Operands = [operand]
            };
        }

        return null; //TODO
    }
    
    private int AddInstructions(Instruction instruction)
    {
        var pos = Instructions.Count;
        Instructions.Add(instruction);
        return pos;
    }
    

    
    private int AddConstant(IReturnableObject obj)
    {
        Constants.Add(obj);
        return Constants.Count - 1;
    }
}