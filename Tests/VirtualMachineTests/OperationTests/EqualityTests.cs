using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using AspectLang.Parser.VirtualMachine.Operations;
using FluentAssertions;

namespace ParserTests.VirtualMachineTests.OperationTests;

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
    
    [Theory]
    [InlineData(true, true, true)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(false, false, true)]
    public void BooleanEqualityAssertions(bool first, bool second, bool outcome)
    {
        var left = new BooleanReturnableObject(first);
        var right = new BooleanReturnableObject(second);
        var instructions = new List<Instruction> { new(OpCode.Equality) };
        var vm = new Vm(instructions, []);
        vm.Push(right);
        vm.Push(left);
        new EqualityOperation().Execute(vm, []);
        var result = vm.Pop();
        result.Should().BeAssignableTo<BooleanReturnableObject>().Which.Value.Should().Be(outcome);
    }
    
    [Theory]
    [InlineData("test", "test", true)]
    [InlineData("Test", "test", false)]
    [InlineData("test string", "test string", true)]
    [InlineData("test    string", "test string", false)]
    [InlineData("$", "$", true)]
    public void StringEqualityAssertions(string first, string second, bool outcome)
    {
        var left = new StringReturnableObject(first);
        var right = new StringReturnableObject(second);
        var instructions = new List<Instruction> { new(OpCode.Equality) };
        var vm = new Vm(instructions, []);
        vm.Push(right);
        vm.Push(left);
        new EqualityOperation().Execute(vm, []);
        var result = vm.Pop();
        result.Should().BeAssignableTo<BooleanReturnableObject>().Which.Value.Should().Be(outcome);
    }
}
