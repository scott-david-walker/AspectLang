using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Compiler;
using FluentAssertions;

namespace ParserTests.CompilerTests;

public class PrefixExpressionTests
{
    [Fact]
    public void MinusPrefix_WhenCompilingInt_ShouldHaveOneInstructionAndOneConstant()
    {
        var compiler = new Compiler();
        var minus = new PrefixExpression("-")
        {
            Right = new IntegerExpression(5)
        };
        compiler.Visit(minus);
        compiler.Instructions.Should().HaveCount(2);
        compiler.Constants.Should().ContainSingle();
        compiler.Instructions[0].OpCode.Should().Be(OpCode.Constant);
        compiler.Instructions[1].OpCode.Should().Be(OpCode.Minus);
    }
    
    [Fact]
    public void ExclamationPrefix_WhenCompilingBoolean_ShouldHaveTwoInstruction()
    {
        var compiler = new Compiler();
        var boolean = new PrefixExpression("!")
        {
            Right = new BooleanExpression(true)
        };
        compiler.Visit(boolean);
        compiler.Instructions.Should().HaveCount(2);
        compiler.Instructions[0].OpCode.Should().Be(OpCode.True);
        compiler.Instructions[1].OpCode.Should().Be(OpCode.Negate);
    }
}