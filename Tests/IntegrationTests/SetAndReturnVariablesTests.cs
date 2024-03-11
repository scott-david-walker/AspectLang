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