namespace ParserTests.IntegrationTests;

public class FunctionTests : TestBase
{
    [Fact]
    public void CanSetVariableFromFunctionReturn()
    {
        var res = Run("fn test(x) {val g = 5; x = 2; return g;} val b = test(1); return b;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(5);
    }
    
    [Fact]
    public void Recursion()
    {
        var res = Run("fn factorial(x) { if(x == 0) { return 1;}  return x * factorial(x - 1); } return factorial(6);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(720);
    }

    [Fact]
    public void MultipleArgumentsSum()
    {
        var res = Run("fn test(x, y) { return x + y; } test(5, 2);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(7);
    }
    
    [Fact]
    public void MultipleArgumentsSubtract()
    {
        var res = Run("fn test(x, y) { return x - y; } test(5, 2);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(3);
    }
    
    [Fact]
    public void MultipleArgumentsMultiply()
    {
        var res = Run("fn test(x, y) { return x * y; } test(5, 2);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(10);
    }
    
    [Fact]
    public void MultipleArgumentsDivide()
    {
        var res = Run("fn test(x, y) { return x / y; } test(6, 2);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(3);
    }

    [Fact]
    public void ThreeArguments()
    {
        var res = Run("fn test(x, y, z) { return x * y * z; } test(3, 2, 4);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(24);
    }
    
    [Fact]
    public void ThreeArgumentsWithPrecedence()
    {
        var res = Run("fn test(x, y, z) { return x + y * z; } test(3, 2, 4);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(11);
    }
    
    [Fact]
    public void ThreeArgumentsWithBrackets()
    {
        var res = Run("fn test(x, y, z) { return (x + y) * z; } test(3, 2, 4);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(20);
    }
    
    [Fact]
    public void StringArguments()
    {
        var res = Run("fn test(x, y) { return x + y; } test(\"scott\", \"here\");");
        res.Should().BeAssignableTo<StringReturnableObject>().Which.Value.Should().Be("scotthere");
    }
    
    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 20)]
    [InlineData(2, 30)]
    [InlineData(3, 40)]
    [InlineData(4, 50)]
    public void CanPassArrayToFunctionAndReturnIndexedValue(int index, int expectedResult)
    {
        var res = Run($"fn test(x) {{ return x[{index}]; }} val n = [10, 20, 30, 40, 50]; return test(n);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(expectedResult);
    }
    
    [Fact]
    public void CanPassMultipleArraysToFunctionAndReturnIndexedValue()
    {
        var res = Run($"fn test(x, y) {{ return y[1]; }} val n = [10, 20, 30, 40, 50]; val x = [5, 15, 25]; return test(n, x);");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(15);
    }
}