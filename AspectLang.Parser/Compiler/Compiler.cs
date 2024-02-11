using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;

namespace AspectLang.Parser.Compiler;

public class Compiler
{
    public List<Instruction> Instructions { get; } = [];
    public List<IReturnableObject> Constants { get; } = [];
    public void Compile(INode node)
    {
        if (node is ProgramNode programNode)
        {
            foreach (var statement in programNode.StatementNodes)
            {
                Compile(statement);
            }
        }

        if (node is ExpressionStatement expressionStatement)
        {
            Compile(expressionStatement.Expression);
        }

        if (node is VariableAssignmentNode valNode)
        {
            Compile(valNode);
        }

        if (node is InfixExpression infixNode)
        {
            Compile(infixNode.Left);
            Compile(infixNode.Right);
            switch (infixNode.Operator)
            {
                case "+":
                    Emit(OpCode.Sum);
                    break;
            }
        }
        
        if (node is IntegerExpression integerExpression)
        {
            var val = integerExpression.Value;
            Emit(OpCode.Constant, [AddConstant(new IntegerReturnableObject(val))]);
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

    private Instruction CreateInstruction(OpCode opCode, List<int> operands)
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