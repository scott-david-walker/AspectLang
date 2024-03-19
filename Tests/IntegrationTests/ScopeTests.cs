using AspectLang.Parser.SemanticAnalysis;

namespace ParserTests.IntegrationTests;

public class ScopeTests : TestBase
{
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
        var parseResult = parser.Parse();
        var result = new SemanticAnalyser().Analyse(parseResult.ProgramNode);
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Variable with name x does not exist");
    }
    
    [Fact]
    public void CannotReachValueStoredInConsequenceBlockInAlternativeBlock()
    {
        var lexer = new Lexer("if(2 == 1) { val x = 1; return x; } else { return x; }");
        var parser = new Parser(lexer);
        var parseResult = parser.Parse();
        var result = new SemanticAnalyser().Analyse(parseResult.ProgramNode);
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
}