namespace ParserTests.IntegrationTests;

public class IfStatementTests : TestBase
{
    [Fact]
    public void CanCompareTwoArrayElements()
    {
        var res = Run("val y = [10]; val g = y; if(g[0] == y[0]) {return 20;}");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(20);
    }
    
    [Fact]
    public void CanCompareTwoArraysDirectly()
    {
        var res = Run("val y = [10]; val g = y; if(g == y) {return 20;}");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(20);
    }
}