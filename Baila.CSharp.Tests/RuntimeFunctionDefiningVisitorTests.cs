using Baila.CSharp.Tests.Infrastructure;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class RuntimeFunctionDefiningVisitorTests : TestsBase
{
    public RuntimeFunctionDefiningVisitorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void FunctionDefinedBeforeCall_Success()
    {
        RunProgram("""
                   function test() {}
                   test();
                   """);
    }

    [Fact]
    public void FunctionDefinedAfterCall_Success()
    {
        RunProgram("""
                   test();
                   function test() {}
                   """);
    }

    [Fact]
    public void FunctionNotDefined_Error()
    {
        RunProgramAndAssertDiagnosticExists(
            "test();",
            d => d.GetErrorMessage() == "'test' is not defined");
    }
}