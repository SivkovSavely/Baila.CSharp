using Baila.CSharp.Ast.Statements;

namespace Baila.CSharp.Visitors;

public class FunctionDefiningVisitor : VisitorBase
{
    public override void VisitFunctionDefineStatement(FunctionDefineStatement stmt)
    {
        base.VisitFunctionDefineStatement(stmt);
        stmt.DefineFunction();
    }
}