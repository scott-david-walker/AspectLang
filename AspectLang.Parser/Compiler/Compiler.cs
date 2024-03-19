using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Ast.Statements;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.SemanticAnalysis;
using AspectLang.Shared;

namespace AspectLang.Parser.Compiler;

public class Loop
{
    public int ConditionPointer { get; set; }
    public int EndPointer { get; set; }
    public int? InstructionToUpdate { get; set; }
}
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
    private readonly FunctionTable _functionTable = new();
    private readonly FunctionTable _functionCalls = new();
    private readonly List<FunctionDeclarationStatement> _functionsDeclarations = [];
    private SemanticAnalysis.SemanticAnalysis _semanticAnalysis;
    private int _scopeCount = 0;
    private Guid? _scopeId = Guid.Parse("2d54b924-5671-408a-8e3d-8d7a25b2043a");
    private Stack<Loop> _loopStack = new();

    public void Compile(INode node)
    {
        node.Accept(this);
        Emit(OpCode.Halt);
        CompileFunctions();
        UpdateCalls();
        // Should do a conversion to some byte code format tbd
    }
    public void Compile(ProgramNode resultProgramNode, SemanticAnalysis.SemanticAnalysis analyse)
    {
        _semanticAnalysis = analyse;
        resultProgramNode.Accept(this);
        Emit(OpCode.Halt);
        CompileFunctions();
        UpdateCalls();
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
            //Entry point is set to the start of the function. This is safe because functions are compiled on the second pass
            var entryPoint = Instructions.Count - 1;
            // could we change all this so that arguments are a different opcode
            
            //double for each because we only want to do a GETLOCAL after the arguments are set
            foreach (var param in function.Parameters)
            {
                var symbol = FindVariableInFunctionScope(function.Name, param.Name);
                
                Emit(OpCode.SetLocal, [new(symbol.Name)]);
            }
            _scopeId = _semanticAnalysis.SymbolTable.Symbols.FirstOrDefault(t => t.Name == function.Name && t.SymbolScope == SymbolScope.Function).ScopeId;
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
        EnterScope();
        foreach (var statement in blockStatement.Statements)
        {
            statement.Accept(this);
        }
        ExitScope();
    }

    private void EnterScope()
    {
        Emit(OpCode.EnterScope);
        _scopeId = _semanticAnalysis.EnterScopes[_scopeCount];
        _scopeCount++;
    }

    private void ExitScope()
    {
        Emit(OpCode.ExitScope);
        _scopeId = _semanticAnalysis.Scopes[_scopeId.Value];
    }
    public void Visit(IfStatement ifStatement)
    {
        ifStatement.Condition.Accept(this);
        var falsePosition = Emit(OpCode.JumpWhenFalse, [new(9999)]);
        ifStatement.Consequence.Accept(this);
        var afterConsequencePosition = Instructions.Count;
        var jumpToEndOfIfInstructionPosition = Emit(OpCode.Jump, [new(9999)]);
        var endPosition = afterConsequencePosition;
        if (ifStatement.Alternative != null)
        {
            ifStatement.Alternative.Accept(this);
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
        var symbol = FindVariableInScope(variableAssignment.VariableDeclarationNode.Name);
        
        Emit(OpCode.SetLocal, [new(symbol.Name)]);
    }

    public void Visit(Identifier identifier)
    {
        var symbol = FindVariableInScope(identifier.Name);
        Emit(OpCode.GetLocal, [new(symbol.Name)]);
    }

    private Symbol FindVariableInScope(string identifier)
    {
        Guid? scopeLevel = _scopeId;
        while (scopeLevel != null)
        {
            var symbol = _semanticAnalysis.SymbolTable.Symbols.FirstOrDefault(t => t.Name == identifier && t.ScopeId == scopeLevel);
            if (symbol != null)
            {
                return symbol;
            }

            scopeLevel = _semanticAnalysis.Scopes[scopeLevel.Value];
        }

        throw new();
    }
    
    private Symbol? FindVariableInFunctionScope(string functionName, string name)
    {
        var symbol = _semanticAnalysis.SymbolTable.Symbols.FirstOrDefault(t => t.Name == functionName && t.SymbolScope == SymbolScope.Function);
        if (symbol != null)
        {
            var scope = symbol.ScopeId;
            symbol = _semanticAnalysis.SymbolTable.Symbols.FirstOrDefault(t => t.Name == name && t.ScopeId == scope);

            return symbol;
        }
        return null;
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

    public void Visit(IterateOverStatement iterateOver)
    {
        Emit(OpCode.Constant, [new(AddConstant(new IntegerReturnableObject(0)))]);
        var startPosition = Instructions.Count - 1;
        Emit(OpCode.SetLocal, [new("index")]);
        Emit(OpCode.Constant, [new(AddConstant(new IntegerReturnableObject(0)))]);
        Emit(OpCode.SetLocal, [new("it")]);
        Emit(OpCode.Compare, [new("index"), new(iterateOver.Identifier.Name)]);
        // var pointer = Emit(OpCode.EndLoop, [new(0)]); 
        // iterateOver.Body.Accept(this);
        // Emit(OpCode.Increment, [new("index")]);
        
        
        var pointer = Emit(OpCode.EndLoop, [new(0)]); 
        var loop = new Loop { ConditionPointer = startPosition + 1 };
        _loopStack.Push(loop);
        iterateOver.Body.Accept(this);
        var incrementPointer = Emit(OpCode.Increment, [new("index")]);
        if (loop.InstructionToUpdate != null)
        {
            loop.EndPointer = incrementPointer - 2; // exit scope as well
            UpdateInstruction(loop.InstructionToUpdate.Value, incrementPointer - 2);
        }

        _loopStack.Pop();
        
        Emit(OpCode.GetLocal, [new("index")]);
        Emit(OpCode.Jump, [new(startPosition)]);
        var endLoop = Instructions.Count - 1;
        UpdateInstruction(pointer, endLoop);
    }

    public void Visit(IterateUntilStatement iterateUntil)
    {
        var startPosition = Instructions.Count - 1;
        iterateUntil.Condition.Accept(this);
        Emit(OpCode.Compare, []);
        var pointer = Emit(OpCode.EndLoop, [new(0)]); 
        iterateUntil.Body.Accept(this);
        Emit(OpCode.Jump, [new(startPosition)]);
        var endLoop = Instructions.Count - 1;
        UpdateInstruction(pointer, endLoop);
    }

    public void Visit(ContinueStatement continueStatement)
    {
        var instruction = Emit(OpCode.Jump, [new(0)]);
        var loop = _loopStack.Pop();
        loop.InstructionToUpdate = instruction;
        _loopStack.Push(loop);
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