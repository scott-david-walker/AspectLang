using AspectLang.Parser.Ast.Statements;

namespace ParserTests.ParserTests;

public class LoopTests : TestBase
{
    [Fact]
    public void CanCreateIterateOverStatement()
    {
        var result = Parse(@"
        val x = [1,2,3]; 
        iterate over x {
            val g = 1; // irrelevant for test but empty block statements are illegal;
        }");

        result.ProgramNode.StatementNodes.Should().HaveCount(2);
        result.ProgramNode.StatementNodes[1].Should().BeAssignableTo<IterateOverStatement>();
    }
    
    [Fact]
    public void GivenParsingIterateOver_WhenNotAnIdentifier_ShouldThrowException()
    {
        var result = Parse(@"
        iterate over 1 == 1 {
            val g = 1; // irrelevant for test but empty block statements are illegal;
        }", false);

        result.Errors.Should().ContainSingle().Which.Message.Should()
            .Be("Expected an identifier but received AspectLang.Parser.Ast.ExpressionTypes.InfixExpression");
    }
    
    [Fact]
    public void CanCreateIterateUntilStatement()
    {
        var result = Parse(@"
        iterate until 1 == 1 {
            val g = 1; // irrelevant for test but empty block statements are illegal;
        }");

        result.ProgramNode.StatementNodes.Should().ContainSingle();
        result.ProgramNode.StatementNodes[0].Should().BeAssignableTo<IterateUntilStatement>();
    }
    
    [Fact]
    public void CanContinue()
    {
        var result = Parse(@"
        val x = [1,2,3]; 
        iterate over x {
            continue;
        }");
        result.ProgramNode.StatementNodes.Should().HaveCount(2);
        result.ProgramNode.StatementNodes[1].Should().BeAssignableTo<IterateOverStatement>();
        var iterate = result.ProgramNode.StatementNodes[1] as IterateOverStatement;
        iterate.Body.Statements.First().Should().BeAssignableTo<ContinueStatement>();
    }
}