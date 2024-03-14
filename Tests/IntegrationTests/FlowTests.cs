using AspectLang.Parser;
using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using FluentAssertions;

namespace ParserTests.IntegrationTests;

public class FlowTests
{
    [Theory]
    [InlineData("5 + 6", 11)]
    [InlineData("10 + 10", 20)]
    [InlineData("10 - 10", 0)]
    [InlineData("6 - 5", 1)]
    [InlineData("6 / 2", 3)]
    [InlineData("15 / 3", 5)]
    [InlineData("15 * 3", 45)]
    [InlineData("6 * 2", 12)]
    [InlineData("6 * 5", 30)]
    [InlineData("6 * 5 / 2", 15)]
    [InlineData("10 * (10 / 2)", 50)]
    [InlineData("2 + 5 * 100 / 10", 52)]
    [InlineData("(2 + 5) * 100 / 10", 70)]
    [InlineData("(2 + (5 + 5)) * 100 / 10", 120)]
    [InlineData("-5 + 6", 1)]
    [InlineData("-5 * -1", 5)]
    [InlineData("5 * -1", -5)]
    [InlineData("-5 * -5", 25)]

    public void SimpleMaths(string source, int expectedResult)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var result = parser.Parse(); 
        var compiler = new Compiler();
        compiler.Compile(result.ProgramNode);

        var vm = new Vm(compiler.Instructions, compiler.Constants);
        var res = vm.Run();
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("1 == 1", true)]
    [InlineData("1 == 2", false)]
    [InlineData("1 != 2", true)]
    [InlineData("1 != 1", false)]
    [InlineData("true == false", false)]
    [InlineData("true == true", true)]
    [InlineData("false == false", true)]
    [InlineData("false != false", false)]
    [InlineData("true != true", false)]
    [InlineData("false != true", true)]
    [InlineData("!false != !true", true)]
    [InlineData("\"test\" == \"test\"", true)]
    [InlineData("\"test\" != \"test2\"", true)]
    [InlineData("!false", true)]
    [InlineData("!true", false)]
    [InlineData("2 > 1", true)]
    [InlineData("1 > 1", false)]
    [InlineData("0 > 1", false)]
    [InlineData("1 >= 1", true)]
    [InlineData("2 < 1", false)]
    [InlineData("1 < 1", false)]
    [InlineData("0 < 1", true)]
    [InlineData("1 <= 1", true)]
    public void EqualityTests(string source, bool expectedResult)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var result = parser.Parse(); 
        var compiler = new Compiler();
        compiler.Compile(result.ProgramNode);
        var vm = new Vm(compiler.Instructions, compiler.Constants);
        var res = vm.Run();
        res.Should().BeAssignableTo<BooleanReturnableObject>().Which.Value.Should().Be(expectedResult);
    }
    
    
    [Theory]
    [InlineData("TestFiles/add.aspect", 15)]
    [InlineData("TestFiles/if.aspect", 4)]
    [InlineData("TestFiles/if_else.aspect", 2)]
    [InlineData("TestFiles/factorial.aspect", 720)]
    public async Task RunTestFiles(string file, int expectedResult)
    {
        var source = await File.ReadAllTextAsync(file);
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var result = parser.Parse(); 
        var compiler = new Compiler();
        compiler.Compile(result.ProgramNode);

        var vm = new Vm(compiler.Instructions, compiler.Constants);
        var res = vm.Run();
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(expectedResult);
    }
}