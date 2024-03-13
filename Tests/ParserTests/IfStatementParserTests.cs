using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;

namespace ParserTests.ParserTests;

public class IfStatementParserTests : TestBase
{
    [Fact]
    public void ParseIfStatement_WithEmptyBlock_ShouldReturnError()
    {
        var result = Parse("if(1 == 1) {}", false);
        result.Errors.Should().ContainSingle();
        var error = result.Errors[0];
        error.Message.Should().Be("Empty block statements are not allowed");
        error.LineNumber.Should().Be(1);
        error.ColumnPosition.Should().Be(12);
    }
    
    [Fact]
    public void CanParseIfStatement_WithConsequence()
    {
        var result = Parse("if(1 == 1) {val x = 0;}");
        var node = result.ProgramNode.StatementNodes[0] as IfStatement;
        node!.Condition.Should()
            .BeAssignableTo<InfixExpression>();
        node.Consequence.Statements.Should().ContainSingle().Which.Should().BeAssignableTo<VariableAssignmentNode>();
    }
    
    [Fact]
    public void CanParseIfStatement_WithConsequence_AndAlternative()
    {
        var result = Parse("if(1 == 1) {val x = 0;} else {val y = 1;}");
        var node = result.ProgramNode.StatementNodes[0] as IfStatement;
        node!.Condition.Should()
            .BeAssignableTo<InfixExpression>();
        node.Consequence.Statements.Should().ContainSingle().Which.Should().BeAssignableTo<VariableAssignmentNode>();
        node.Alternative!.Statements.Should().ContainSingle().Which.Should().BeAssignableTo<VariableAssignmentNode>();
    }
}