using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using AspectLang.Parser.VirtualMachine.Operations;
using FluentAssertions;

namespace ParserTests.VirtualMachineTests;

public class NotEqualTests
{
    [Theory]
    [InlineData(5, 5, false)]
    [InlineData(2, 99, true)]
    [InlineData(40, 1234, true)]
    public void IntegerEqualityAssertions(int number1, int number2, bool outcome)
    {
        var left = new IntegerReturnableObject(number1);
        var right = new IntegerReturnableObject(number2);
        var instructions = new List<Instruction> { new(OpCode.Equality) };
        var vm = new Vm(instructions, []);
        vm.Push(right);
        vm.Push(left);
        new NotEqualOperation().Execute(vm, []);
        var result = vm.Pop();
        result.Should().BeAssignableTo<BooleanReturnableObject>().Which.Value.Should().Be(outcome);
    }
}