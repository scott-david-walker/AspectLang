using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;

namespace AspectLang.Parser.Compiler;

public class Compiler
{
    public List<byte> Instructions { get; } = [];
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

        if (node is ExpressionStatement statement2)
        {
            Compile(statement2.Expression);
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
    
    private int AddInstructions(IEnumerable<byte> instructions)
    {
        var pos = Instructions.Count;
        Instructions.AddRange(instructions);
        return pos;
    }
    
    private int Emit(OpCode opcode, List<int> operands)
    {
        var instructions = ByteCode.Create(opcode, operands);
        var position =  AddInstructions(instructions);
        //SetLastInstruction((byte)opcode, position);
        return position;
    }
    
    private int AddConstant(IReturnableObject obj)
    {
        Constants.Add(obj);
        return Constants.Count - 1;
    }
}