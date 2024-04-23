using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class StringValueExpressionInterpreter : ExpressionInterpreterBase<StringValueExpression>
{
    public override IValue Interpret(StringValueExpression expression)
    {
        return new StringValue(expression.Value);
    }
}