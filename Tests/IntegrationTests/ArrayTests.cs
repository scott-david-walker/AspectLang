namespace ParserTests.IntegrationTests;

public class ArrayTests : TestBase
{ 
    [Fact]
    public void CanGetIndex()
    {
        var res = Run($"val y = [10]; return y[0];");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(10);
    }
    
    [Fact]
    public void CanCopyArrayValueIntoNewVariableAndChangeValue()
    {
        var res = Run($"val y = [10]; val x = y[0]; x = 100; return x;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(100);
    }
    
    [Fact]
    public void CanCopyArrayValueIntoNewVariableAndChangeValueWithoutAdjustingOriginalArray()
    {
        var res = Run($"val y = [10]; val x = y[0]; x = 100; return y[0];");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(10);
    }
}