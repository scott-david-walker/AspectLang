using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Ast.Statements;
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
    private readonly List<FunctionDeclarationStatement> _functionsDeclarations = [];

    public void Compile(INode node)
    {
        node.Accept(this);
        Emit(OpCode.Halt);
        CompileFunctions();
        UpdateCalls();
        // Should do a conversion to some byte code format tbd
    }

    private void UpdateCalls()
    {
        foreach (var functionCall in _functionCalls.Functions)
        {
            var functionDeclaration = _functionTable.Functions.FirstOrDefault(t =>
                t.Name == functionCall.Name && t.ParametersCount == functionCall.ParametersCount);

            if (functionDeclaration == null)
            {
                throw new("FUNCTION NOT FOUND");
            }

            var currentLocation = functionCall.EntryPoint;
            // jump location
            Instructions[currentLocation].Operands[0].Reference = functionDeclaration.EntryPoint;
            //return location
            Instructions[currentLocation].Operands[1].Reference = currentLocation;
        }
    }
    private void CompileFunctions()
    {
        foreach (var function in _functionsDeclarations)
        {
            EnterScope();
            //Entry point is set to the start of the function. This is safe because functions are compiled on the second pass
            var entryPoint = Instructions.Count - 1;
            // could we change all this so that arguments are a different opcode
            
            //double for each because we only want to do a GETLOCAL after the arguments are set
            foreach (var param in function.Parameters)
            {
                var symbol = _scope.SymbolTable.Define(param.Name);
                
                Emit(OpCode.SetLocal, [new(symbol.Name)]);
            }
            foreach (var param in function.Parameters)
            {
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
        Emit(OpCode.Constant, [new(AddConstant(new IntegerReturnableObject(val)))]);
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
            case "<":
                Emit(OpCode.LessThan);
                break;
            case "<=":
                Emit(OpCode.LessThanEqualTo);
                break;
            case ">":
                Emit(OpCode.GreaterThan);
                break;
            case ">=":
                Emit(OpCode.GreaterThanEqualTo);
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
        Emit(OpCode.Constant, [new(AddConstant(new StringReturnableObject(val)))]);
    }

    public void Visit(BlockStatement blockStatement)
    {
        foreach (var statement in blockStatement.Statements)
        {
            statement.Accept(this);
        }
    }

    private void EnterScope()
    {
        Emit(OpCode.EnterScope);
        var scope = new Scope(_scope);
        _scope = scope;
    }

    private void ExitScope()
    {
        _scope = _scope.Parent;
        Emit(OpCode.ExitScope);
    }
    public void Visit(IfStatement ifStatement)
    {
        ifStatement.Condition.Accept(this);
        var falsePosition = Emit(OpCode.JumpWhenFalse, [new(9999)]);
        EnterScope();
        ifStatement.Consequence.Accept(this);
        ExitScope();
        var afterConsequencePosition = Instructions.Count;
        var jumpToEndOfIfInstructionPosition = Emit(OpCode.Jump, [new(9999)]);
        var endPosition = afterConsequencePosition;
        if (ifStatement.Alternative != null)
        {
            EnterScope();
            ifStatement.Alternative.Accept(this);
            ExitScope();
            endPosition = Instructions.Count;
            UpdateInstruction(falsePosition, afterConsequencePosition);
        }
        else
        {
            UpdateInstruction(falsePosition, endPosition);
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
        
        Emit(OpCode.SetLocal, [new(symbol.Name)]);
    }

    public void Visit(Identifier identifier)
    {
        var symbol = FindVariableInScope(identifier.Name);
        Emit(OpCode.GetLocal, [new(symbol.Name)]);
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
        _functionsDeclarations.Add(functionDeclaration);
    }

    public void Visit(FunctionCall functionCall)
    {
        functionCall.Args.ForEach(arg =>
        {
            arg.Accept(this);
        });
        
        var location = Emit(OpCode.JumpToFunction, [new(0), new(0), new(functionCall.Args.Count)]);
        _functionCalls.Functions.Add(new()
        {
            Name = functionCall.Name,
            ParametersCount = functionCall.Args.Count,
            EntryPoint = location,
            ReturnPoint = Instructions.Count // This is a bit of a hack but it is updated on the second pass.
        });
    }

    public void Visit(ArrayLiteral array)
    {
        foreach (var element in array.Elements)
        {
            element.Accept(this);
        }

        Emit(OpCode.Array, [new(array.Elements.Count)]);
    }

    public void Visit(IndexExpression indexExpression)
    {
        indexExpression.Left.Accept(this);
        indexExpression.Index.Accept(this);
        Emit(OpCode.Index);
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
    
    private int Emit(OpCode opcode, List<Operand> operands)
    {
        var instruction = CreateInstruction(opcode, operands);
        var position = AddInstructions(instruction);
        return position;
    }

    private static Instruction CreateInstruction(OpCode opCode, List<Operand> operands)
    {
        if (!operands.Any())
        {
            return new() { OpCode = opCode };
        }

        return new()
        {
            OpCode = opCode,
            Operands = operands
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