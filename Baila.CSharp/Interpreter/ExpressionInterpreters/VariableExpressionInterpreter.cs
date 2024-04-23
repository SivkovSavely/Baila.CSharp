using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class VariableExpressionInterpreter : ExpressionInterpreterBase<VariableExpression>
{
    public override IValue Interpret(VariableExpression expression)
    {
        var name = expression.Name;
        var member = NameTable.Get(name);
        return member.Value;
    }
}