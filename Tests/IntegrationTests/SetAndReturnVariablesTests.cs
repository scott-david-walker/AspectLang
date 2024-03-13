
namespace ParserTests.IntegrationTests;

public class SetAndReturnVariablesTests : TestBase
{
    [Fact]
    public void CanSetVariable()
    {
        var res = Run("val x = 5; return x;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(5);
    }
    
    [Fact]
    public void CanReassignVariable()
    {
        var res = Run("val x = 5; x = 10; return x;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(10);
    }
    
    [Fact]
    public void CanCreateNewVariableWithOldVariableAssignment()
    {
        var res = Run("val x = 5; val y = 1; val z = y; return z;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(1);
    }
    
    [Fact]
    public void CanCreateNewVariableWithOldVariableAssignment_ButStillReturnOriginalVariable()
    {
        var res = Run("val x = 5; val y = 1; val z = y; return y;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(1);
    }
    
    [Fact]
    public void ShouldNotOverwriteVariable()
    {
        var res = Run("val x = 5; if(1 == 1) { val g = x; g = 6; } return x;");
        res.Should().BeAssignableTo<IntegerReturnableObject>().Which.Value.Should().Be(5);
    }
}