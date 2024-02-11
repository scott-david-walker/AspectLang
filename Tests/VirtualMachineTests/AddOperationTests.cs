using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using FluentAssertions;

namespace ParserTests.VirtualMachineTests;

public class AddOperationTests
{
    [Theory]
    [InlineData(5, 5, 10)]
    [InlineData(2, 99, 101)]
    [InlineData(40, 1234, 1274)]
    public void CanAddTwoNumbers(int number1, int number2, int outcome)
    {
        var left = new IntegerReturnableObject(number1);
        var right = new IntegerReturnableObject(number2);
        var instructions = new List<Instruction> { new(OpCode.Sum) };
        var vm = new Vm(instructions, []);
        vm.Push(right);
        vm.Push(left);
        new AddOperation().Execute(vm, []);
        var result = vm.Pop();
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(outcome);
    }
}