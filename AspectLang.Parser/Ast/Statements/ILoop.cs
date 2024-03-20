namespace AspectLang.Parser.Ast.Statements;

public interface ILoop
{
    BlockStatement Body { get; set; }
}