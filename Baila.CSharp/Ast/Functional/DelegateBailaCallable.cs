using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Ast.Functional;

public class DelegateBailaCallable(Func<BailaCallableArgs, IValue> func) : IBailaCallable
{
    public IValue Call(BailaCallableArgs args)
    {
        return func(args);
    }
}

public class DelegateVoidBailaCallable(Action<BailaCallableArgs> func) : IBailaCallable
{
    public IValue? Call(BailaCallableArgs args)
    {
        func(args);
        return null;
    }
}