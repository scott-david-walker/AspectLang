namespace ParserTests.IntegrationTests.LoopTests;

public class ContinueTests : TestBase
{
        
    [Fact]
    public void CanContinueIterationInUntil()
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
    
    [Fact]
    public void CanContinueIterationInOver()
    {
        var result = Run(@"
            val x = [1,2,3]; 
            val g = 0;
            iterate over x {
                continue;
                g = 1;
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(0);
    }
    
    [Fact]
    public void CanContinueIterationInNestedLoop()
    {
        var result = Run(@"
            val x = [1,2,3]; 
            val y = [10,20,30]; 
            val g = 0;
            iterate over x {
                g = it;
                iterate over y {
                    continue;
                    g = 10000;
                }
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(3);
    }
    
    [Fact]
    public void CanContinueInFirstOfNestedLoops()
    {
        var result = Run(@"
            val x = [1,2,3]; 
            val y = [10,20,30]; 
            val g = 0;
            iterate over x {
                continue;
                iterate over y {
                    g = it;
                }
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(0);
    }
}