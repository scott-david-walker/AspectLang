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
            case "-":
                Emit(OpCode.Subtract);
                break;
            case "/":
                Emit(OpCode.Divide);
                break;
            case "*":
                Emit(OpCode.Multiply);
                break;
            case "==":
                Emit(OpCode.Equality);
                break;
            case "!=":
                Emit(OpCode.NotEqual);
                break;
        }
    }

    public void Visit(PrefixExpression expression)
    {
        expression.Right.Accept(this);
        switch (expression.Operator)
        {
            case "-":
                Emit(OpCode.Minus);
                break;
            case "!":
                Emit(OpCode.Negate);
                break;
        }
    }

    public void Visit(BooleanExpression expression)
    {
        Emit(expression.Value ? OpCode.True : OpCode.False);
    }

    public void Visit(StringExpression expression)
    {
        var val = expression.Value;
        Emit(OpCode.Constant, [AddConstant(new StringReturnableObject(val))]);
    }

    public void Visit(BlockStatement blockStatement)
    {
        foreach (var statement in blockStatement.Statements)
        {
            statement.Accept(this);
        }
    }

    public void Visit(IfStatement ifStatement)
    {
        ifStatement.Condition.Accept(this);
        var falsePosition = Emit(OpCode.JumpWhenFalse, [9999]);
        ifStatement.Consequence.Accept(this);
        var afterConsequencePosition = Instructions.Count;
        var jumpToEndOfIfInstructionPosition = Emit(OpCode.Jump, [9999]);
        var endPosition = afterConsequencePosition;
        if (ifStatement.Alternative != null)
        {
            ifStatement.Alternative.Accept(this);
            endPosition = Instructions.Count;
            UpdateInstruction(falsePosition, afterConsequencePosition);
        }
        UpdateInstruction(jumpToEndOfIfInstructionPosition, endPosition);
    }

    private void UpdateInstruction(int position, int location)
    {
        var instruction = Instructions[position];
        instruction.Operands[0].Reference = location;
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

        if (opCode is OpCode.Constant or OpCode.JumpWhenFalse or OpCode.Jump)
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