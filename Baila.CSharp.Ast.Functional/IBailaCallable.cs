using System.Dynamic;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Ast.Functional;

public interface IBailaCallable
{
    IValue? Call(BailaCallableArgs args);
}