using AspectLang.Parser;
using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using FluentAssertions;

namespace ParserTests;

public class ParserTests
{
    [Fact]
    public void CanParseValStatement()
    {
        var lexer = new Lexer("val x = 5;");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.ProgramNode.StatementNodes[0].Should().BeAssignableTo<VariableAssignmentNode>();
        var node = result.ProgramNode.StatementNodes[0] as VariableAssignmentNode;
        node!.VariableDeclarationNode.Name.Should().Be("x");
        node!.Expression.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
    }

    [Fact]
    public void WhenNextStatementIsNotAssignment_ShouldReturnError()
    {
        var lexer = new Lexer("val x 5;");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().NotBeEmpty();
        var error = result.Errors[0];
        error.ColumnPosition.Should().Be(6);
        error.LineNumber.Should().Be(0);
        error.Message.Should().Be("Expected = but received 5");
    }
    
    [Fact]
    public void WhenNextStatementIsNotIdentifier_ShouldReturnError()
    {
        var lexer = new Lexer("val 5 = 5;");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        result.Errors.Should().NotBeEmpty();
        var error = result.Errors[0];
        error.ColumnPosition.Should().Be(4);
        error.LineNumber.Should().Be(0);
        error.Message.Should().Be("Expected identifier but received 5");
    }

    [Fact]
    public void ValStatementRighthandSideIsExpression()
    {
        var lexer = new Lexer("val x = 5;");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as VariableAssignmentNode;
        node!.Expression.Should()
            .BeAssignableTo<IntegerExpression>()
            .Which.Value.Should()
            .Be(5);
    }
    
    [Fact]
    public void ExpressionCanHandlePlus()
    {
        var lexer = new Lexer("val x = 5 + 5;");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as VariableAssignmentNode;
        node!.Expression.Should()
            .BeAssignableTo<InfixExpression>();

        var infix = node.Expression as InfixExpression;
        infix!.Operator.Should().Be("+"); 
        infix.Left.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
        infix.Right.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
    }
    
    [Fact]
    public void ExpressionCanHandleMinus()
    {
        var lexer = new Lexer("val x = 5 - 5;");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as VariableAssignmentNode;
        node!.Expression.Should()
            .BeAssignableTo<InfixExpression>();

        var infix = node.Expression as InfixExpression;
        infix!.Operator.Should().Be("-"); 
        infix.Left.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
        infix.Right.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
    }
    
    
    [Fact]
    public void ExpressionCanHandleTimes()
    {
        var lexer = new Lexer("val x = 5 * 5;");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as VariableAssignmentNode;
        node!.Expression.Should()
            .BeAssignableTo<InfixExpression>();

        var infix = node.Expression as InfixExpression;
        infix!.Operator.Should().Be("*"); 
        infix.Left.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
        infix.Right.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
    }
    
    
    [Fact]
    public void ExpressionCanHandleDivide()
    {
        var lexer = new Lexer("val x = 5 / 5;");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as VariableAssignmentNode;
        node!.Expression.Should()
            .BeAssignableTo<InfixExpression>();

        var infix = node.Expression as InfixExpression;
        infix!.Operator.Should().Be("/"); 
        infix.Left.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
        infix.Right.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
    }
    
    [Fact]
    public void ExpressionCanHandleOrderOfOperations()
    {
        var lexer = new Lexer("val x = 5 + 5 / 10;");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as VariableAssignmentNode;
        node!.Expression.Should()
            .BeAssignableTo<InfixExpression>();

        var leftInfix = node.Expression as InfixExpression;
        leftInfix!.Operator.Should().Be("+"); 
        leftInfix.Left.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
        leftInfix.Right.Should().BeAssignableTo<InfixExpression>();

        var rightInfix = leftInfix.Right as InfixExpression;
        rightInfix!.Operator.Should().Be("/");
        rightInfix.Left.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
        rightInfix.Right.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(10);
    }
    
    [Fact]
    public void ExpressionCanHandleOrderOfOperationsWithBrackets()
    {
        var lexer = new Lexer("val x = (5 + 5) / 10;");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as VariableAssignmentNode;
        node!.Expression.Should()
            .BeAssignableTo<InfixExpression>();

        var infix = node.Expression as InfixExpression;
        infix!.Operator.Should().Be("/"); 
        infix.Left.Should().BeAssignableTo<InfixExpression>();
        var leftInfix = infix.Left as InfixExpression;
        leftInfix!.Left.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
        leftInfix.Right.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(5);
        leftInfix.Operator.Should().Be("+");
        infix.Right.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(10);
    }
    
    [Fact]
    public void Minus()
    {
        var lexer = new Lexer("-20");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as ExpressionStatement;
        node!.Expression.Should()
            .BeAssignableTo<PrefixExpression>();

        var prefix = node.Expression as PrefixExpression;
        prefix!.Operator.Should().Be("-");
        prefix.Right.Should().BeAssignableTo<IntegerExpression>().Which.Value.Should().Be(20);
    }
    
    [Fact]
    public void Negate()
    {
        var lexer = new Lexer("!true");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as ExpressionStatement;
        node!.Expression.Should()
            .BeAssignableTo<PrefixExpression>();

        var prefix = node.Expression as PrefixExpression;
        prefix!.Operator.Should().Be("!");
        prefix.Right.Should().BeAssignableTo<BooleanExpression>().Which.Value.Should().Be(true);
    }
    
    [Fact]
    public void Equality()
    {
        var lexer = new Lexer("20 == 20");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as ExpressionStatement;
        node!.Expression.Should()
            .BeAssignableTo<InfixExpression>();

        var infix = node.Expression as InfixExpression;
        infix!.Operator.Should().Be("==");
    }
    
    [Fact]
    public void NotEqual()
    {
        var lexer = new Lexer("20 != 20");
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as ExpressionStatement;
        node!.Expression.Should()
            .BeAssignableTo<InfixExpression>();

        var infix = node.Expression as InfixExpression;
        infix!.Operator.Should().Be("!=");
    }
    
    [Fact]
    public void CanParseString()
    {
        var lexer = new Lexer("\"Test String\";"); // 1 is there as parser expects multiple tokens
        var parser = new Parser(lexer);
        var result = parser.Parse();
        var node = result.ProgramNode.StatementNodes[0] as ExpressionStatement;
        node!.Expression.Should()
            .BeAssignableTo<StringExpression>();
        var stringExpression = node.Expression as StringExpression;
        stringExpression!.Value.Should().Be("Test String");
    }

    // [Theory]
    // [InlineData("true", true)]
    // [InlineData("false", false)]
    // public void CanHandleBooleanValues(string source, bool returnValue)
    // {
    //     var lexer = new Lexer(source);
    //     var parser = new Parser(lexer);
    //     var result = parser.Parse();
    //     var node = result.ProgramNode.StatementNodes[0] as ExpressionStatement;
    //     node!.Expression.Should()
    //         .BeAssignableTo<BooleanExpression>();
    // }
}