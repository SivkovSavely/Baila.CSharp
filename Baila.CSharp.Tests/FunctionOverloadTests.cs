using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Tests.Infrastructure;
using FluentAssertions;
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
        var functionsCalled = new List<string>();

        NameTable.AddConstant(
            "testFuncWithOneInt",
            FunctionValue.WithOverload(
                new FunctionOverload(args => { functionsCalled.Add("testFuncWithOneInt"); }, [], null)));

        RunProgram("""
                   function testFunc(x: Int) : Int
                   {
                       testFuncWithOneInt()
                   }

                   testFunc(123)
                   """);

        functionsCalled.Should().Equal("testFuncWithOneInt");
    }

    [Fact]
    public void FunctionWithOneOverload_OverloadResolutionSuccessfulForSupertypeParameter()
    {
        var functionsCalled = new List<string>();

        NameTable.AddConstant(
            "testFuncWithOneNumber",
            FunctionValue.WithOverload(
                new FunctionOverload(args => { functionsCalled.Add("testFuncWithOneNumber"); }, [], null)));

        RunProgram("""
                   function testFunc(x: Number) : Int
                   {
                       testFuncWithOneNumber()
                   }

                   var reallyAnInt: Int = 123
                   testFunc(reallyAnInt)
                   """);

        functionsCalled.Should().Equal("testFuncWithOneNumber");
    }

    [Fact]
    public void FunctionWithTwoOverloads_DifferByParameterCount_CallSuccessful()
    {
        var functionsCalled = new List<string>();

        NameTable.AddConstant(
            "testFuncWithOneInt",
            FunctionValue.WithOverload(
                new FunctionOverload(args => { functionsCalled.Add("testFuncWithOneInt"); }, [], null)));

        NameTable.AddConstant(
            "testFuncWithTwoInts",
            FunctionValue.WithOverload(
                new FunctionOverload(args => { functionsCalled.Add("testFuncWithTwoInts"); }, [], null)));

        RunProgram("""
                   function testFunc(x: Int) : Int
                   {
                     return testFuncWithOneInt()
                   }
                   function testFunc(x: Int, y: Int) : Int
                   {
                     return testFuncWithTwoInts()
                   }

                   testFunc(123)
                   testFunc(123, 456)
                   """);

        functionsCalled.Should().Equal("testFuncWithOneInt", "testFuncWithTwoInts");
    }

    [Fact]
    public void FunctionWithOneOverload_DefaultParameter_CallSuccessful()
    {
        var numbers = new List<long>();

        NameTable.AddConstant(
            "notifyTestAboutNumber",
            FunctionValue.WithOverload(
                new FunctionOverload(args => { numbers.Add(args.GetInteger(0)); },
                    [new FunctionParameter("number", BailaType.Int)], null)));

        RunProgram("""
                   function testFunc(x: Int = 5) : Int
                   {
                       notifyTestAboutNumber(x)
                   }

                   testFunc()
                   testFunc(123)
                   """);

        numbers.Should().Equal(5, 123);
    }

    [Fact]
    public void FunctionWithOneOverload_RequiredParameterAfterOptionalParameter_CompileError()
    {
        CompileProgramAndAssertDiagnosticExists(
            """
            function testFunc(x: Int = 5, y: Int) : Int
            {
            }
            """,
            diagnostic => diagnostic.GetErrorMessage() ==
                          "in function 'testFunc', required parameter 'y' cannot be after an optional parameter 'x'");
    }

    [Fact]
    public void FunctionWithTwoOverloads_RequiredParameterAfterOptionalParameter_CompileError()
    {
        CompileProgramAndAssertDiagnosticExists(
            """
            function testFunc() : Int
            {
            }
            function testFunc(x: Int = 5, y: Int) : Int
            {
            }
            """,
            diagnostic => diagnostic.GetErrorMessage() ==
                          "in function 'testFunc', required parameter 'y' cannot be after an optional parameter 'x'");
    }

    [Fact]
    public void ComplexOverloads_SuccessfullyDefined()
    {
        CompileProgram("""
                       function overloadTest(a: Any) {}
                       function overloadTest() {}
                       function overloadTest(a: Int) {}
                       function overloadTest(a: Float) {}
                       function overloadTest(a: Float, b: Float) {}
                       """);
    }

    [Fact]
    public void ComplexOverloads_SameOverloadTwice_CompileError()
    {
        CompileProgramAndAssertDiagnosticExists("""
                                                function overloadTest(a: Any) {}
                                                function overloadTest() {}
                                                function overloadTest(a: Int) {}
                                                function overloadTest(a: Float) {}
                                                function overloadTest(a: Float) {}
                                                function overloadTest(a: Float, b: Float) {}
                                                """,
            d => d.GetErrorMessage() == "overload (a: Float) conflicts with overload (a: Float)"
        );
    }

    [Fact]
    public void ComplexOverloads_ConflictingOverloads_CompileError()
    {
        CompileProgramAndAssertDiagnosticExists("""
                                                function overloadTest(a: Any) {}
                                                function overloadTest() {}
                                                function overloadTest(a: Int) {}
                                                function overloadTest(a: Float) {}
                                                function overloadTest(a: Float, b: Float = 1) {}
                                                """,
            d => d.GetErrorMessage() == "overload (a: Float[,b: Float]) conflicts with overload (a: Float)"
        );
    }
}