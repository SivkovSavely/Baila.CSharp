using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class PrefixUnaryExpressionInterpreter : ExpressionInterpreterBase<PrefixUnaryExpression>
{
    public override IValue Interpret(PrefixUnaryExpression expression)
    {
        throw new NotImplementedException();
    }
}