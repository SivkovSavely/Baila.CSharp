using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Values;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class ValueExpressionInterpreter : ExpressionInterpreterBase<ValueExpression>
{
    public override IValue Interpret(ValueExpression expression)
    {
        throw new NotImplementedException();
    }
}