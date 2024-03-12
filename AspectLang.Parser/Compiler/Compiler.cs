using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Shared;

namespace AspectLang.Parser.Compiler;

public class FunctionTable
{
    public List<Function> Functions { get; set; } = [];
}

public class Function
{
    public string Name { get; set; }
    public int ParametersCount { get; set; }
    public int EntryPoint { get; set; }
    public int ReturnPoint { get; set; }
}

public class Compiler : IVisitor
{
    public List<Instruction> Instructions { get; } = [];
    public List<IReturnableObject> Constants { get; } = [];
    private Scope _scope = new(null);
    private readonly FunctionTable _functionTable = new();
    private readonly FunctionTable _functionCalls = new();
    private readonly List<FunctionDeclarationStatement> _functions = [];

    public void Compile(INode node)
    {
        node.Accept(this);
        
        CompileFunctions();
        UpdateCalls();
        // Should do a conversion to some byte code format tbd
    }

    private void UpdateCalls()
    {
        foreach (var function in _functionCalls.Functions)
        {
            var f = _functionTable.Functions.FirstOrDefault(t =>
                t.Name == function.Name && t.ParametersCount == function.ParametersCount);

            if (f == null)
            {
                throw new("FUNCTION NOT FOUND");
            }

            var currentLocation = function.EntryPoint;
            Instructions[currentLocation].Operands[0].Reference = f.EntryPoint;
        }
    }
    private void CompileFunctions()
    {
        foreach (var function in _functions)
        {
            var entryPoint = Instructions.Count - 1;
            foreach (var param in function.Parameters)
            {
                var symbol = _scope.SymbolTable.Define(param.Name);
                Emit(OpCode.SetLocal, [symbol.Index]);
                param.Accept(this);
            }
            function.Body.Accept(this);
            _functionTable.Functions.Add(new()
            {
                Name = function.Name,
                ParametersCount = function.Parameters.Count,
                EntryPoint = entryPoint,
                ReturnPoint = Instructions.Count
            });
        }
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
        Emit(OpCode.EnterScope);
        var scope = new Scope(_scope);
        _scope = scope;
        foreach (var statement in blockStatement.Statements)
        {
            statement.Accept(this);
        }

        _scope = scope.Parent;
        Emit(OpCode.ExitScope);
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

    public void Visit(VariableDeclarationNode variableDeclaration)
    {
        variableDeclaration.Accept(this);
    }

    public void Visit(VariableAssignmentNode variableAssignment)
    {
        variableAssignment.Expression.Accept(this);
        Symbol symbol;
        if (variableAssignment.VariableDeclarationNode.IsFreshDeclaration)
        {
            symbol = _scope.SymbolTable.Define(variableAssignment.VariableDeclarationNode.Name);
        }
        else
        {
            symbol = FindVariableInScope(variableAssignment.VariableDeclarationNode.Name);
        }
        
        Emit(OpCode.SetLocal, [symbol.Index]);
    }

    public void Visit(Identifier identifier)
    {
        var symbol = FindVariableInScope(identifier.Name);
        Emit(OpCode.GetLocal, [symbol.Index]);
    }

    private Symbol FindVariableInScope(string identifier)
    {
        var scope = _scope;
        while (scope != null)
        {
            if (scope.SymbolTable.Exists(identifier))
            {
                break;
            }
            scope = scope.Parent;
        }
        var symbol = scope.SymbolTable.Resolve(identifier);
        return symbol;
    }

    public void Visit(ReturnStatement returnStatement)
    {
        returnStatement.Value.Accept(this);
        Emit(OpCode.Return);
    }

    public void Visit(FunctionDeclarationStatement functionDeclaration)
    {
        _functions.Add(functionDeclaration);
    }

    public void Visit(FunctionCall functionCall)
    {
        Emit(OpCode.EnterScope);
        functionCall.Args.ForEach(arg =>
        {
            arg.Accept(this);
        });
        
        var location = Emit(OpCode.JumpToFunction, [0]);
        _functionCalls.Functions.Add(new()
        {
            Name = functionCall.Name,
            ParametersCount = functionCall.Args.Count,
            EntryPoint = location,
            ReturnPoint = Instructions.Count
        });
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

        //TODO: This is horrid
        var length = opCode.FindLength();
        if (length == 0)
        {
            throw new("Expected opcode to have memory allocated for operands");
        }
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