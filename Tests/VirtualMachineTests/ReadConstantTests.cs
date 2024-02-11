using AspectLang.Parser.Compiler;
using AspectLang.Parser.Compiler.ReturnableObjects;
using AspectLang.Parser.VirtualMachine;
using FluentAssertions;

namespace ParserTests.VirtualMachineTests;

public class ReadConstantTests
{
    [Fact]
    public void ReadConstantShouldAddConstantToStack()
    {
        var left = new IntegerReturnableObject(100);
        var constants = new List<IReturnableObject> { left };
        var instructions = new List<Instruction> { new(OpCode.Constant) };
        var vm = new Vm(instructions, constants);
        new ReadConstantOperation().Execute(vm, [new() { OperandType = OperandType.Pointer, Reference = 0}]);
        var result = vm.Pop();
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(100);
    }
}