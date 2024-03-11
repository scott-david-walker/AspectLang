using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using AspectLang.Parser.VirtualMachine.Operations;
using FluentAssertions;

namespace ParserTests.VirtualMachineTests.OperationTests;


public class SubtractOperationTests
{
    [Theory]
    [InlineData(5, 5, 0)]
    [InlineData(2, 99, -97)]
    [InlineData(1234, 40, 1194)]
    public void CanAddTwoNumbers(int number1, int number2, int outcome)
    {
        var left = new IntegerReturnableObject(number1);
        var right = new IntegerReturnableObject(number2);
        var instructions = new List<Instruction> { new(OpCode.Subtract) };
        var vm = new Vm(instructions, []);
        vm.Push(left);
        vm.Push(right);
        new SubtractOperation().Execute(vm, []);
        var result = vm.Pop();
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(outcome);
    }
}