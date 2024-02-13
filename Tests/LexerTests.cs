using AspectLang.Parser;
using FluentAssertions;

namespace ParserTests;

public class LexerTests
{
    [Fact]
    public void OnInstantiation_ShouldBeLineZeroAndColumnZeroAndEndOfFile()
    {
        var lexer = new Lexer("");
        var token = lexer.NextToken();
        token.TokenType.Should().Be(TokenType.EndOfFile);
        token.LineNumber.Should().Be(0);
        token.ColumnPosition.Should().Be(0);
        token.Literal.Should().BeNull();
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void WhenNewLine_ShouldIncrementLineNumber(string source)
    {
        var lexer = new Lexer(source);
        var token = lexer.NextToken();
        token.LineNumber.Should().Be(1);
    }

    [Fact]
    public void WhenSourceContainsTab_ShouldIncrementByFourColumns()
    {
        var lexer = new Lexer("\t");
        var token = lexer.NextToken();
        token.ColumnPosition.Should().Be(4);
    }

    [Theory]
    [InlineData("val", TokenType.Val)]
    [InlineData("x", TokenType.Identifier)]
    [InlineData("1", TokenType.Integer)]
    [InlineData("x1", TokenType.Identifier)]
    [InlineData("x_1", TokenType.Identifier)]
    [InlineData(";", TokenType.SemiColon)]
    [InlineData("*", TokenType.Asterisk)]
    [InlineData("+", TokenType.Plus)]
    [InlineData("-", TokenType.Minus)]
    [InlineData("/", TokenType.Slash)]
    [InlineData("(", TokenType.LeftParen)]
    [InlineData(")", TokenType.RightParen)]
    [InlineData("true", TokenType.True)]
    [InlineData("false", TokenType.False)]
    [InlineData("==", TokenType.Equality)]
    public void ShouldReturnCorrectIdentifier(string source, TokenType tokenType)
    {
        var lexer = new Lexer(source);
        var token = lexer.NextToken();
        token.TokenType.Should().Be(tokenType);
        token.Literal.Should().Be(source); // lazy but I'm only evaluating a single token
    }
}