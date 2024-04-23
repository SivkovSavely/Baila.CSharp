using Baila.CSharp.Ast.Expressions;

namespace Baila.CSharp.Ast.Statements;

public class ForStatement(
    string counterVariable,
    IExpression initialValue,
    IExpression finalValue,
    IExpression stepValue,
    IStatement body)
    : IStatement
{
    public string CounterVariable { get; } = counterVariable;
    public IExpression InitialValue { get; } = initialValue;
    public IExpression FinalValue { get; } = finalValue;
    public IExpression StepValue { get; } = stepValue;
    public IStatement Body { get; } = body;

    public override string ToString()
    {
        return $"ForStatement(" +
               $"CounterVariable={CounterVariable}, " +
               $"InitialValue={InitialValue}, " +
               $"FinalValue = {FinalValue}, " +
               $"StepValue = {StepValue}, " +
               $"Body = {Body})";
    }
}