using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Compiler;
using FluentAssertions;

namespace ParserTests.CompilerTests;

public class InfixExpressionTests
{
    [Theory]
    [InlineData("+", OpCode.Sum)]
    [InlineData("-", OpCode.Subtract)]
    [InlineData("/", OpCode.Divide)]
    [InlineData("*", OpCode.Multiply)]
    [InlineData("==", OpCode.Equality)]
    public void OutputCorrectConstantsAndOperator(string @operator, OpCode opCode)
    {
        var infix = new InfixExpression();
        var right = new IntegerExpression(1);
        var left = new IntegerExpression(1);
        infix.Operator = @operator;
        infix.Right = right;
        infix.Left = left;

        var compiler = new Compiler();
        compiler.Visit(infix);
        var instructions = compiler.Instructions;
        instructions.Should().HaveCount(3);
        instructions[0].OpCode.Should().Be(OpCode.Constant);
        instructions[1].OpCode.Should().Be(OpCode.Constant);
        instructions[2].OpCode.Should().Be(opCode);
    }
}