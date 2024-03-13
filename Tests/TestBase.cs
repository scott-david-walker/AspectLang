using AspectLang.Parser.Ast;
using AspectLang.Parser.Compiler;
using AspectLang.Parser.VirtualMachine;

namespace ParserTests;

public abstract class TestBase
{
    protected static ParseResult Parse(string source)
    {
        var result = Parse(source, true);
        return result;
    }
    
    protected static ParseResult Parse(string source, bool withErrors)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var result = parser.Parse();
        if (withErrors)
        {
            result.Errors.Should().BeEmpty();
        }
        return result;
    }
    
    protected static IReturnableObject Run(string source)
    {
        var result = Parse(source);
        var compiler = new Compiler();
        compiler.Compile(result.ProgramNode);
        var vm = new Vm(compiler.Instructions, compiler.Constants);
        return vm.Run();
    }
}