using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using AspectLang.Parser.VirtualMachine.Operations;
using FluentAssertions;

namespace ParserTests.VirtualMachineTests.OperationTests;

public class MinusOperationTests
{
    [Theory]
    [InlineData(5, -5)]
    [InlineData(2, -2)]
    [InlineData(40, -40)]   
    [InlineData(-5, 5)]
    [InlineData(-2, 2)]
    [InlineData(-40, 40)]
    public void CanNegateAnInteger(int num, int outcome)
    {
        var integerObject = new IntegerReturnableObject(num); 
        var vm = new Vm([], []);
        vm.Push(integerObject);
        new MinusOperation().Execute(vm, []);
        var result = vm.Pop();
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(outcome);
    }
}