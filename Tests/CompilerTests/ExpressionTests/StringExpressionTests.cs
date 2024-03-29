using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using FluentAssertions;

namespace ParserTests.CompilerTests.ExpressionTests;

public class StringExpressionTests
{
    [Fact]
    public void WhenCompilingString_ShouldHaveOneInstructionAndOneConstant()
    {
        var compiler = new Compiler();
        compiler.Visit(new AspectLang.Parser.Ast.ExpressionTypes.StringExpression("Test String"));
        compiler.Instructions.Should().ContainSingle();
        compiler.Constants.Should().ContainSingle();
        var instruction = compiler.Instructions[0];
        instruction.OpCode.Should().Be(OpCode.Constant);
        instruction.Operands.Should().ContainSingle();
        instruction.Operands[0].OperandType.Should().Be(OperandType.Pointer);
        instruction.Operands[0].Reference.Should().Be(0);
        var stringObject = compiler.Constants[0] as StringReturnableObject;
        stringObject.Should().NotBeNull();
        stringObject!.Value.Should().Be("Test String");
    }
}