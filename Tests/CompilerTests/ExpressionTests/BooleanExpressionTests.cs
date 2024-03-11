using AspectLang.Parser;
using AspectLang.Parser.Compiler;
using FluentAssertions;

namespace ParserTests.CompilerTests.ExpressionTests;

public class BooleanExpressionTests
{
    [Fact]
    public void WhenCompilingBoolean_ShouldHaveOneInstruction()
    {
        var compiler = new Compiler();
        compiler.Visit(new AspectLang.Parser.Ast.ExpressionTypes.BooleanExpression(true, new ("irrelevant", 0, 0, TokenType.False)));
        compiler.Instructions.Should().ContainSingle();
        var instruction = compiler.Instructions[0];
        instruction.OpCode.Should().Be(OpCode.True);
    }
}

