using AspectLang.Parser.VirtualMachine;
using FluentAssertions;

namespace ParserTests.CompilerTests;

public class SymbolTableTests
{
    [Fact]
    public void CanAddSymbol()
    {
        var symbolTable = new SymbolTable();
        var symbol = symbolTable.Define("test");
        symbol.Index.Should().Be(0);
        symbol.Name.Should().Be("test");
        symbol.Scope.Should().Be(SymbolScope.Global);
    }
    
    [Fact]
    public void GivenSymbolTableHasSymbol_WhenAddingASecondSymbol_ShouldReturnCorrectIndex()
    {
        var symbolTable = new SymbolTable();
        _ = symbolTable.Define("test");
        var symbol = symbolTable.Define("another");
        symbol.Index.Should().Be(1);
        symbol.Name.Should().Be("another");
        symbol.Scope.Should().Be(SymbolScope.Global);
    }
}