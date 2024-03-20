using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;
using AspectLang.Parser.Ast.Statements;

namespace AspectLang.Parser.Compiler;

public interface IVisitor
{
    void Visit(IntegerExpression expression);
    void Visit(ProgramNode node);
    void Visit(ExpressionStatement expression);
    void Visit(InfixExpression expression);
    void Visit(PrefixExpression expression);
    void Visit(BooleanExpression expression);
    void Visit(StringExpression expression);
    void Visit(BlockStatement blockStatement);
    void Visit(IfStatement ifStatement);
    void Visit(VariableDeclarationNode variableDeclaration);
    void Visit(VariableAssignmentNode variableAssignment);
    void Visit(Identifier identifier);
    void Visit(ReturnStatement returnStatement);
    void Visit(FunctionDeclarationStatement functionDeclaration);
    void Visit(FunctionCall functionCall);
    void Visit(ArrayLiteral array);
    void Visit(IndexExpression indexExpression);
    void Visit(IterateOverStatement iterateOver);
    void Visit(IterateUntilStatement iterateUntil);
    void Visit(ContinueStatement continueStatement);
    void Visit(BreakStatement breakStatement);
}
