using AspectLang.Parser;
using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using FluentAssertions;

namespace ParserTests.ParserTests;

public class FunctionParserTests
{
    [Fact]
    public void CanParseNoArgumentFunction()
    {
        var lexer = new Lexer("fn test() {val x = 0;}"); 
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().BeEmpty();
        result.ProgramNode.StatementNodes[0].Should().BeAssignableTo<FunctionDeclarationStatement>();
        var node = result.ProgramNode.StatementNodes[0] as FunctionDeclarationStatement;
        node!.Name.Should().Be("test");
        node.Arguments.Should().BeEmpty();
        node.Body.Statements.Should().ContainSingle().Which.Should().BeAssignableTo<VariableAssignmentNode>();
    }
    
    [Fact]
    public void CanParseSingleArgumentFunction()
    {
        var lexer = new Lexer(@"fn test(1) {val x = 0;}"); 
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().BeEmpty();
        result.ProgramNode.StatementNodes[0].Should().BeAssignableTo<FunctionDeclarationStatement>();
        var node = result.ProgramNode.StatementNodes[0] as FunctionDeclarationStatement;
        node!.Name.Should().Be("test");
        node.Arguments[0].Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(1);
        node.Body.Statements.Should().ContainSingle().Which.Should().BeAssignableTo<VariableAssignmentNode>();
    }
    
    [Fact]
    public void CanParseMultipleArgumentFunction()
    {
        var lexer = new Lexer("fn test(1, \"string arg\") {val x = 0;}"); 
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().BeEmpty();
        result.ProgramNode.StatementNodes[0].Should().BeAssignableTo<FunctionDeclarationStatement>();
        var node = result.ProgramNode.StatementNodes[0] as FunctionDeclarationStatement;
        node!.Name.Should().Be("test");
        node.Arguments.Should().HaveCount(2);
        node.Arguments[0].Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(1);
        node.Arguments[1].Should().BeAssignableTo<StringExpression>().Which.Value.Should().Be("string arg");
        node.Body.Statements.Should().ContainSingle().Which.Should().BeAssignableTo<VariableAssignmentNode>();
    }
    
}