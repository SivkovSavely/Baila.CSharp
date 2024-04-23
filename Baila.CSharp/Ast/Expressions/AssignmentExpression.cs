using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Expressions;

public class AssignmentExpression(string target, IExpression expression) : IExpression
{
    public BailaType? GetBailaType()
    {
        return expression.GetBailaType();
    }

    public string Stringify()
    {
        return $"{target} = {expression.Stringify()}";
    }

    public override string ToString()
    {
        return $"AssignmentExpression({target} = {expression})";
    }
}