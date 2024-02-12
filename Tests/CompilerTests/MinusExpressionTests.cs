using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Compiler;
using FluentAssertions;

namespace ParserTests.CompilerTests;

public class MinusExpressionTests
{
    [Fact]
    public void WhenCompilingInt_ShouldHaveOneInstructionAndOneConstant()
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
}