using Baila.CSharp.Tests.Infrastructure;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class FunctionOverloadTests(ITestOutputHelper testOutputHelper) : TestsBase
{
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