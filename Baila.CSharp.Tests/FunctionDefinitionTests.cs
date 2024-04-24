using Baila.CSharp.Tests.Infrastructure;

namespace Baila.CSharp.Tests;

public class FunctionDefinitionTests : TestsBase
{
    [Fact]
    public void BraceOnOtherLine_ShouldParse()
    {
        CompileProgram("""
                       function test() : Int
                       {
                       }
                       """);
    }

    [Fact]
    public void BraceOnSameLine_ShouldParse()
    {
        CompileProgram("""
                       function test() : Int {
                       }
                       """);
    }

    [Fact]
    public void ReturnTypeOnOtherLine1_ShouldParse()
    {
        CompileProgram("""
                       function test()
                           : Int {}
                       """);
    }

    [Fact]
    public void ReturnTypeOnOtherLine2_ShouldParse()
    {
        CompileProgram("""
                       function test() :
                           Int {}
                       """);
    }

    [Fact]
    public void NoParametersNoParentheses_ShouldParse()
    {
        CompileProgram("""
                       function test : Int {}
                       """);
    }
}