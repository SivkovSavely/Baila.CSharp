using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Tests.Infrastructure;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class FunctionOverloadTests : TestsBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public FunctionOverloadTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        testOutputHelper.WriteLine("Run ctor in FunctionOverloadTests");
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
    public void FunctionWithOneOverload_OverloadResolutionSuccessfulForSupertypeParameter()
    {
        RunProgram("""
                   function testFunc(x: Number) : Int
                   {
                   }

                   var reallyAnInt: Int = 123
                   testFunc(reallyAnInt)
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