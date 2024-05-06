using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Runtime.Types;

namespace Baila.CSharp.Ast.Functional;

public record FunctionParameter(string Name, BailaType Type, IExpression? DefaultValue = null, bool Vararg = false)
{
    public override string ToString()
    {
        return (Vararg ? "..." : "") +
               (Name + ": " + Type) +
               (DefaultValue != null ? DefaultValue.Stringify() : "");
    }
}