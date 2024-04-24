using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Tests.Infrastructure;

namespace Baila.CSharp.Tests;

public class FunctionOverloadTests : TestsBase
{
    public FunctionOverloadTests()
    {
        NameTable.CurrentScope = new NameTable.Scope();
    }
    
    [Fact]
    public void FunctionWithOneOverload_CallSuccessful()
    {
        RunProgram("""
                   function testFunc(x: Int) : Int
                   {
                   }

                   testFunc(123)
                   """);
    }

    [Fact]
    public void FunctionWithTwoOverloads_DifferByParameterCount_CallSuccessful()
    {
        RunProgram("""
                   function testFunc(x: Int) : Int
                   {
                     return x
                   }
                   function testFunc(x: Int, y: Int) : Int
                   {
                     return y
                   }

                   testFunc(123)
                   testFunc(123, 456)
                   """);
    }
}