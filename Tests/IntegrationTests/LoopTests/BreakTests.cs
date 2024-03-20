namespace ParserTests.IntegrationTests.LoopTests;

public class BreakTests : TestBase
{
    [Fact]
    public void CanBreakIterationInUntil()
    {
        var result = Run(@"
            val x = 0;
            val g = 10;
            iterate until x == 2 {
                x = x + 1;
                break;
                g = x;
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(10);
    }
    
    [Fact]
    public void CanBreakIterationInOver()
    {
        var result = Run(@"
            val x = [1,2,3]; 
            val g = 0;
            iterate over x {
                break;
                g = 1;
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(0);
    }
    
    [Fact]
    public void CanBreakIterationInNestedLoop()
    {
        var result = Run(@"
            val x = [1,2,3]; 
            val y = [10,20,30]; 
            val g = 0;
            iterate over x {
                g = it;
                iterate over y {
                    break;
                    g = 10000;
                }
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(3);
    }
    
    [Fact]
    public void CanBreakInFirstOfNestedLoops()
    {
        var result = Run(@"
            val x = [1,2,3]; 
            val y = [10,20,30]; 
            val g = 0;
            iterate over x {
                break;
                iterate over y {
                    g = it;
                }
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(0);
    }
}