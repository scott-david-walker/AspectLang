using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using AspectLang.Parser.VirtualMachine.Operations;
using FluentAssertions;

namespace ParserTests.VirtualMachineTests.OperationTests;

public class DivideOperationTests
{
    [Theory]
    [InlineData(10, 5, 2)]
    [InlineData(100, 10, 10)]
    [InlineData(50, 2, 25)]
    public void CanAddTwoNumbers(int number1, int number2, int outcome)
    {
        var left = new IntegerReturnableObject(number1);
        var right = new IntegerReturnableObject(number2);
        var instructions = new List<Instruction> { new(OpCode.Divide) };
        var vm = new Vm(instructions, []);
        vm.Push(left);
        vm.Push(right);
        new DivideOperation().Execute(vm, []);
        var result = vm.Pop();
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(outcome);
    }
}