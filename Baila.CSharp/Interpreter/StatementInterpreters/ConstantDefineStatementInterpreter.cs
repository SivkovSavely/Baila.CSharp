using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.ExpressionInterpreters;
using Baila.CSharp.Interpreter.Stdlib;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class ConstantDefineStatementInterpreter : StatementInterpreterBase<ConstantDefineStatement>
{
    public override void Interpret(ConstantDefineStatement statement)
    {
        NameTable.AddConstant(statement.Name, statement.Value.InterpretEvaluate());
    }
}