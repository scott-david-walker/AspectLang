using AspectLang.Parser.Ast;
using AspectLang.Parser.Ast.ExpressionTypes;

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
}
