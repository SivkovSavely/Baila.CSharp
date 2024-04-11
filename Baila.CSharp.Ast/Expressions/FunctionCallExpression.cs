namespace Baila.CSharp.Ast.Expressions;

public class FunctionCallExpression(IExpression functionHolder, List<IExpression> callArgs) : IExpression
{
    public IExpression FunctionHolder { get; } = functionHolder;
    public List<IExpression> CallArgs { get; } = callArgs;

    public string Stringify()
    {
        return $"{FunctionHolder.Stringify()}({string.Join(", ", CallArgs.Select(x => x.Stringify()))})";
    }
}