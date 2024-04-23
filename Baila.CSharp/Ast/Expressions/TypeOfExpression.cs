using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Expressions;

public class TypeOfExpression(IExpression expr) : IExpression
{
    public BailaType? GetBailaType()
    {
        return expr.GetBailaType();
    }

    public IValue Evaluate()
    {
        if (expr is VariableExpression variableExpression)
        {
            return new StringValue(NameTable.Get(variableExpression.Name).Type.ToString());
        }

        return new StringValue(expr.GetBailaType()!.ToString());
    }

    public string Stringify()
    {
        return expr.Stringify();
    }

    public override string ToString()
    {
        return $"TypeOfExpression({expr})";
    }
}