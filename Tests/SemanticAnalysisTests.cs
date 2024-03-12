using AspectLang.Parser;
using FluentAssertions;

namespace ParserTests;

public class SemanticAnalysisTests
{
    [Fact]
    public void WhenReferencingIdentifierThatWasNotSet_ShouldThrowError()
    {
        var lexer = new Lexer("x = 5;"); 
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Variable with name x does not exist");
    }
    
    [Fact]
    public void WhenDeclaringVariableThatHasAlreadyBeenSet_ShouldThrowError()
    {
        var lexer = new Lexer("val x = 5; val x = 10;"); 
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Variable with name x already exists");
    }
    
    [Fact]
    public void WhenReferencingIdentifierThatSetInHigherScope_ShouldNotThrowError()
    {
        var lexer = new Lexer("val x = 5; if(5 == 5) { x = 10; }"); 
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void WhenDeclaringVariableThatWasSetInHigherScope_ShouldThrowError()
    {
        var lexer = new Lexer("val x = 5; if(5 == 5) { val x = 10; }"); 
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Variable with name x already exists");
    }
}