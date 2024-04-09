namespace Baila.CSharp.Ast.Expressions;

public interface IExpression
{
    /// <summary>
    /// Returns the Baila code for this expression.
    /// The code returned by this method should be able to be parsed back into the identical expression.
    /// </summary>
    /// <returns>Stringified expression</returns>
    string Stringify();
}