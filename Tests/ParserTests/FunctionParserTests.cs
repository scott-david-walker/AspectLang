using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Ast.Statements;

namespace ParserTests.ParserTests;

public class FunctionParserTests : TestBase
{
    [Fact]
    public void CanParseNoArgumentFunction()
    {
        var result = Parse("fn test() {val x = 0;}"); 
        result.ProgramNode.StatementNodes[0].Should().BeAssignableTo<FunctionDeclarationStatement>();
        var node = result.ProgramNode.StatementNodes[0] as FunctionDeclarationStatement;
        node!.Name.Should().Be("test");
        node.Parameters.Should().BeEmpty();
        node.Body.Statements.Should().ContainSingle().Which.Should().BeAssignableTo<VariableAssignmentNode>();
    }
    
    [Fact]
    public void CanParseSingleArgumentFunction()
    {
        var result = Parse("fn test(y) {val x = 0;}"); 
        result.ProgramNode.StatementNodes[0].Should().BeAssignableTo<FunctionDeclarationStatement>();
        var node = result.ProgramNode.StatementNodes[0] as FunctionDeclarationStatement;
        node!.Name.Should().Be("test");
        node.Parameters[0].Should().BeAssignableTo<Identifier>().Which.Name.Should().Be("y");
        node.Body.Statements.Should().ContainSingle().Which.Should().BeAssignableTo<VariableAssignmentNode>();
    }
    
    [Fact]
    public void CanParseMultipleArgumentFunction()
    {
        var result = Parse("fn test(x, y) {val x = 0;}"); 
        result.ProgramNode.StatementNodes[0].Should().BeAssignableTo<FunctionDeclarationStatement>();
        var node = result.ProgramNode.StatementNodes[0] as FunctionDeclarationStatement;
        node!.Name.Should().Be("test");
        node.Parameters.Should().HaveCount(2);
        node.Parameters[0].Should().BeAssignableTo<Identifier>().Which.Name.Should().Be("x");
        node.Parameters[1].Should().BeAssignableTo<Identifier>().Which.Name.Should().Be("y");
        node.Body.Statements.Should().ContainSingle().Which.Should().BeAssignableTo<VariableAssignmentNode>();
    }

    [Fact]
    public void CanParseCall()
    {
        var result = Parse("test(1,2,3);"); 
        result.ProgramNode.StatementNodes[0].Should().BeAssignableTo<ExpressionStatement>();
        var expressionStatement = result.ProgramNode.StatementNodes[0] as ExpressionStatement;
        var functionCall = expressionStatement.Expression as FunctionCall;
        functionCall!.Name.Should().Be("test");
        functionCall.Args.Should().HaveCount(3);
        functionCall.Args[0].Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(1);
        functionCall.Args[1].Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(2);
        functionCall.Args[2].Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(3);
    }
}