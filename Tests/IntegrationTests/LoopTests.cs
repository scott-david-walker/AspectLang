namespace ParserTests.IntegrationTests;

public class LoopTests : TestBase
{
    [Fact]
    public void CanLoopOverArrayAndReturnValue()
    {
        var result = Run(@"
        val x = [1,2,3]; 
        val g = 5;
        iterate over x {
            g = it; 
        }
        return g;");

        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(3);
    }
    
    [Fact]
    public void WhenArrayIsEmpty_ShouldSkipLoop()
    {
        var result = Run(@"
        val x = []; 
        val g = 5;
        iterate over x {
            g = it; 
        }
        return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(5);
    }
    
    [Fact]
    public void NestedLoop()
    {
        var result = Run(@"
        val x = [1,2,3]; 
        val y = [10,20,30]; 
        val g = 0;
        iterate over x {
            iterate over y {
                g = it; 
            }
        }
        return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(30);
    }
    
    [Fact]
    public void NestedLoopWithDifferentLengths()
    {
        var result = Run(@"
        val x = [1,2,3,4]; 
        val y = [10,20]; 
        val g = 0;
        iterate over x {
            iterate over y {
                g = it; 
            }
        }
        return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(20);
    }

    [Fact]
    public void WhenInDoubleLoop_CanSetItToADifferentValueInOuterLoop_AndReturn()
    {
        var result = Run(@"
            val x = [1,2,3]; 
            val g = 0;
            val q = 10;
            iterate over x {
                if(it == 2) {
                    g = it;
                }
                iterate over x {
                    q = 1;
                }
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(2);
    }

    [Fact]
    public void CanReturnIndex()
    {
        var result = Run(@"
            val x = [1,2,3]; 
            val g = 0;
            iterate over x {
                if(it == 2) {
                    g = index;
                }
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(1);
    }
    
    [Fact]
    public void CanReturnIndexInSeparateLoop()
    {
        var result = Run(@"
            val x = [1,2,3]; 
            val g = 0;
            iterate over x {
                if(it == 2) {
                    val p = 1;
                }
                iterate over x {
                    if(it == 1) {
                        g = index;
                    }
                }
            }
            return g;");
        result.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(0);
    }
}