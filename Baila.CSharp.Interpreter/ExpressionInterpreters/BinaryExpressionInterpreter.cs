using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class BinaryExpressionInterpreter : ExpressionInterpreterBase<BinaryExpression>
{
    public override IValue Interpret(BinaryExpression expression)
    {
        throw new NotImplementedException();
    }
}