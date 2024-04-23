using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Functional;

public class FunctionOverload(IBailaCallable callback, List<FunctionParameter> parameters, BailaType? returnType)
{
    public IBailaCallable Callback { get; } = callback;
    public List<FunctionParameter> Parameters { get; } = parameters;
    public BailaType? ReturnType { get; } = returnType;

    public FunctionOverload(
        Func<BailaCallableArgs, IValue> callback,
        List<FunctionParameter> parameters,
        BailaType? returnType) : this(new DelegateBailaCallable(callback), parameters, returnType)
    {
    }

    public FunctionOverload(
        Action<BailaCallableArgs> callback,
        List<FunctionParameter> parameters,
        BailaType? returnType) : this(new DelegateVoidBailaCallable(callback), parameters, returnType)
    {
    }
}