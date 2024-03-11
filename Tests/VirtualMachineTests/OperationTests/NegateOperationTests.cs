using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using AspectLang.Parser.VirtualMachine.Operations;
using FluentAssertions;

namespace ParserTests.VirtualMachineTests.OperationTests;

public class NegateOperationTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanNegateBoolean(bool vmValue)
    {
        var boolean = new BooleanReturnableObject(vmValue);
        var instructions = new List<Instruction> { new(OpCode.Negate) };
        var vm = new Vm(instructions, []);
        vm.Push(boolean);
        new NegateOperation().Execute(vm, []);
        var result = vm.Pop();
        result.Should().BeAssignableTo<BooleanReturnableObject>().Which.Value.Should().Be(!vmValue);
    }

    [Fact]
    public void WhenOperationIsNotBoolean_ShouldThrowException()
    {
        var integer = new IntegerReturnableObject(1);
        var instructions = new List<Instruction> { new(OpCode.Negate) };
        var vm = new Vm(instructions, []);
        vm.Push(integer);
        var operation = new NegateOperation();
        var func = () => operation.Execute(vm, []);
        func.Should().ThrowExactly<Exception>().WithMessage("Expected Boolean in negate");
    }
}