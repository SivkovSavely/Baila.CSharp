using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Interpreter.ExpressionInterpreters;

public class AssignmentExpressionInterpreter : ExpressionInterpreterBase<AssignmentExpression>
{
    public override IValue Interpret(AssignmentExpression expression)
    {
        throw new NotImplementedException();
    }
}