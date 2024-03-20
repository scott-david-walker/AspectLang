namespace ParserTests.IntegrationTests.LoopTests;

public class IterateUntilTests : TestBase
{
    [Fact]
    public void WhenExpressionIsTrue_ShouldSkipLoop()
    {
        var result = Run(@"
            val x = 0;
            iterate until 1 == 1 {
                x = 10;
            }
            return x;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(0);
    }
    
    [Fact]
    public void WhenExpressionIsFalse_ShouldEnterLoop()
    {
        var result = Run(@"
            val x = 0;
            iterate until x == 1 {
                x = x + 1;
            }
            return x;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(1);
    }
    
    [Fact]
    public void ExpressionCanBeAgainstArrays()
    {
        var result = Run(@"
            val x = 0;
            val y = [1,2,3];
            iterate until x == y[1] {
                x = x + 1;
            }
            return x;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(2);
    }
    
    [Fact]
    public void CanContinueIteration()
    {
        var result = Run(@"
            val x = 0;
            val g = 10;
            iterate until x == 2 {
                x = x + 1;
                continue;
                g = x;
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(10);
    }
}