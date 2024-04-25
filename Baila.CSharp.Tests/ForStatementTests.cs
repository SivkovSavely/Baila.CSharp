using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Tests.Infrastructure;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class ForStatementTests : TestsBase
{
    public ForStatementTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void X()
    {
        var program = CompileProgram("""
                                     for i = 1 to 3 {}
                                     """);

        Assert.False(NameTable.Exists("i"), "Loop counter should not exist at this point in time");
        program.Execute();
        Assert.False(NameTable.Exists("i"), "Loop counter should not be exposed to the scope after loop ran");
    }
}