using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Ast.Functional;

public interface IBailaCallable
{
    IValue? Call(BailaCallableArgs args);
}