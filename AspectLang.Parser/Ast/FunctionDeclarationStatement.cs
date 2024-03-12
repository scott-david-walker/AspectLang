using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public class FunctionDeclarationStatement : IStatement
{
    public Token Token { get; set; }
    public string Name { get; set; }
    public List<IExpression> Arguments = [];
    public BlockStatement Body { get; set; }
    public void Accept(IVisitor visitor)
    {
        
    }
}