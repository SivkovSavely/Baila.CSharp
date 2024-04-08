using Baila.CSharp.Ast.Values;

namespace Baila.CSharp.Ast.Expressions;

public interface IExpression
{
    /// <summary>
    /// Evaluates the expression and returns the result
    /// of the evaluation in the form of a Baila value.
    /// </summary>
    /// <returns>Result of the expression evaluation</returns>
    IValue Evaluate();

    /// <summary>
    /// Returns the Baila code for this expression.
    /// The code returned by this method should be able to be parsed back into the identical expression.
    /// </summary>
    /// <returns>Stringified expression</returns>
    string Stringify();
}