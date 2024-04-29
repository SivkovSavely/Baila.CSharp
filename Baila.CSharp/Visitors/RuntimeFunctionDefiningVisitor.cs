using Baila.CSharp.Ast.Syntax.Statements;

namespace Baila.CSharp.Visitors;

public class RuntimeFunctionDefiningVisitor : VisitorBase
{
    public override void VisitFunctionDefineStatement(FunctionDefineStatement stmt)
    {
        base.VisitFunctionDefineStatement(stmt);
        stmt.DefineFunction();
    }
}