using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.ExpressionInterpreters;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Interpreter.StatementInterpreters;

public class VariableDefineStatementInterpreter : StatementInterpreterBase<VariableDefineStatement>
{
    public override void Interpret(VariableDefineStatement statement)
    {
        if (statement.Type == null)
        {
            // Infer from the value
            if (statement.Value == null)
            {
                throw new Exception(
                    $"Error: either type or value should be provided for the variable {statement.Name}");
            }

            var evaled = statement.Value.InterpretEvaluate();
            NameTable.AddVariable(statement.Name, evaled.GetBailaType(), evaled);
            return;
        }

        if (statement.Value == null)
        {
            // Infer default value from the type
            NameTable.AddVariable(statement.Name, statement.Type, statement.Type.GetDefaultValue());
            return;
        }
        
        NameTable.AddVariable(statement.Name, statement.Type, statement.Value.InterpretEvaluate());
    }
}