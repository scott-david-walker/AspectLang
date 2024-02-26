using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using AspectLang.Parser.VirtualMachine.Operations;
using FluentAssertions;

namespace ParserTests.VirtualMachineTests;

public class EqualityTests
{
    [Theory]
    [InlineData(5, 5, true)]
    [InlineData(2, 99, false)]
    [InlineData(40, 1234, false)]
    public void IntegerEqualityAssertions(int number1, int number2, bool outcome)
    {
        var left = new IntegerReturnableObject(number1);
        var right = new IntegerReturnableObject(number2);
        var instructions = new List<Instruction> { new(OpCode.Equality) };
        var vm = new Vm(instructions, []);
        vm.Push(right);
        vm.Push(left);
        new EqualityOperation().Execute(vm, []);
        var result = vm.Pop();
        result.Should().BeAssignableTo<BooleanReturnableObject>().Which.Value.Should().Be(outcome);
    }
}