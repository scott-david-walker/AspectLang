using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using FluentAssertions;

namespace ParserTests.CompilerTests;

public class IntegerExpressionTests
{
    [Fact]
    public void WhenCompilingInt_ShouldHaveOneInstructionAndOneConstant()
    {
        var compiler = new Compiler();
        compiler.Visit(new AspectLang.Parser.Ast.ExpressionTypes.IntegerExpression(5));
        compiler.Instructions.Should().ContainSingle();
        compiler.Constants.Should().ContainSingle();
        var instruction = compiler.Instructions[0];
        instruction.OpCode.Should().Be(OpCode.Constant);
        instruction.Operands.Should().ContainSingle();
        instruction.Operands[0].OperandType.Should().Be(OperandType.Pointer);
        instruction.Operands[0].Reference.Should().Be(0);
        var intObject = compiler.Constants[0] as IntegerReturnableObject;
        intObject.Should().NotBeNull();
        intObject!.Value.Should().Be(5);
    }
}