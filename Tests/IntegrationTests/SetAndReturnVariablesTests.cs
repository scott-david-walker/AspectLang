using AspectLang.Parser;
using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using FluentAssertions;

namespace ParserTests.IntegrationTests;

public class SetAndReturnVariablesTests
{
    [Fact]
    public void CanSetVariable()
    {
        var res = Run("val x = 5; return x;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(5);
    }
    
    
    [Fact]
    public void CanReassignVariable()
    {
        var res = Run("val x = 5; x = 10; return x;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(10);
    }
    
    [Fact]
    public void CanCreateNewVariableWithOldVariableAssignment()
    {
        var res = Run("val x = 5; val y = 1; val z = y; return z;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(1);
    }
    
    [Fact]
    public void CanCreateNewVariableWithOldVariableAssignment_ButStillReturnOriginalVariable()
    {
        var res = Run("val x = 5; val y = 1; val z = y; return y;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(1);
    }
    
    [Fact]
    public void CanReachVariablesFromWithinInnerScope()
    {
        var res = Run("val x = 5; if(x == 5) { x = 1; return x; }");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(1);
    }
    
    [Fact]
    public void CanReachVariablesFromWithinInnerScopeWithinElseStatement()
    {
        var res = Run("val x = 5; val y = 10; if(x == 1) { x = 1; return x; } else { return y; }");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(10);
    }

    [Fact]
    public void CannotReachValueStoredInIfStatementIfNotPreviouslySet()
    {
        var lexer = new Lexer("if(1 == 1) { x = 1; return x; }");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Variable with name x does not exist");
    }
    
    [Fact]
    public void CannotReachValueStoredInConsequenceBlockInAlternativeBlock()
    {
        var lexer = new Lexer("if(2 == 1) { val x = 1; return x; } else { return x; }");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Variable with name x does not exist");
    }

    [Fact]
    public void CanReferenceVariablesInNestedIfs()
    {
        var res = Run("val x = 5; if(1 == 1) { if(2 == 2) { return x;} }");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(5);
    }

    [Fact]
    public void WillReturnCorrectValueFromNestedIfs()
    {
        var res = Run("val x = 5; if(1 == 1) { x = 100; if(2 == 2) { if(3 == 3) { return x; } } } ");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(100);
    }
    
    [Fact]
    public void NestedBlock()
    {
        var res = Run("if(1 == 1) { val x = 100; if(2 == 2) { return x; } val g = 5;} ");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(100);
    }

    [Fact]
    public void Test()
    {
        var res = Run("fn test(x) {val g = 5; x = 2; return g;} val b = test(1); return b;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(5);
    }
    
    [Fact]
    public void Scope()
    {
        var res = Run("val x = 5; if(1 == 1) { val g = x; g = 6; } return x;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(5);
    }
    
    [Fact]
    public void Recursion()
    {
        var res = Run("fn factorial(x) { if(x == 0) { return 1;}  return x * factorial(x - 1); } return factorial(6);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(720);
    }

    private static IReturnableObject Run(string source)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().BeEmpty();
        var compiler = new Compiler();
        compiler.Compile(result.ProgramNode);
        var vm = new Vm(compiler.Instructions, compiler.Constants);
        return vm.Run();
    }
}