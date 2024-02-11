using AspectLang.Parser.Compiler;

namespace AspectLang.Parser.Ast;

public interface INode
{
    void Accept(IVisitor visitor);
}