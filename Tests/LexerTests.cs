using AspectLang.Parser;
using FluentAssertions;

namespace ParserTests;

public class LexerTests
{
    [Fact]
    public void OnInstantiation_ShouldBeLineOneAndColumnZeroAndEndOfFile()
    {
        var lexer = new Lexer("");
        var token = lexer.NextToken();
        token.TokenType.Should().Be(TokenType.EndOfFile);
        token.LineNumber.Should().Be(1);
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
        token.LineNumber.Should().Be(2);
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
    [InlineData("!=", TokenType.NotEqual)]
    [InlineData("!", TokenType.Exclamation)]
    [InlineData("if", TokenType.If)]
    [InlineData("else", TokenType.Else)]
    [InlineData("{", TokenType.LeftCurly)]
    [InlineData("}", TokenType.RightCurly)]
    [InlineData("return", TokenType.Return)]
    [InlineData("fn", TokenType.Function)]
    [InlineData(",", TokenType.Comma)]
    [InlineData("<", TokenType.LessThan)]
    [InlineData(">", TokenType.GreaterThan)]
    [InlineData(">=", TokenType.GreaterThanEqualTo)]
    [InlineData("<=", TokenType.LessThanEqualTo)]
    [InlineData("[", TokenType.LeftSquareBracket)]
    [InlineData("]", TokenType.RightSquareBracket)]
    public void ShouldReturnCorrectIdentifier(string source, TokenType tokenType)
    {
        var lexer = new Lexer(source);
        var token = lexer.NextToken();
        token.TokenType.Should().Be(tokenType);
        token.Literal.Should().Be(source); // lazy but I'm only evaluating a single token
    }

    [Fact]
    public void ShouldReturnString()
    {
        var lexer = new Lexer("\"test string\"");
        var token = lexer.NextToken();
        token.TokenType.Should().Be(TokenType.String);
        token.Literal.Should().Be("test string"); 
    }

    [Fact]
    public void ShouldIgnoreComments()
    {
        var lexer = new Lexer(@"
// This is a comment
val x = ");
        var token = lexer.NextToken();
        token.TokenType.Should().Be(TokenType.Val);
    }
    
    [Fact]
    public void ShouldIgnoreMultipleComments()
    {
        var lexer = new Lexer(@"
// This is a comment
// And this is another

// And another
val x = ");
        var token = lexer.NextToken();
        token.TokenType.Should().Be(TokenType.Val);
    }
}