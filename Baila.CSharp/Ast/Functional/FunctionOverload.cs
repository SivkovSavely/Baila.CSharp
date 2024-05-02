using System.Text;
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

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append('(');

        for (int i = 0; i < Parameters.Count; i++)
        {
            var delimiter = "";
            if (i > 0)
            {
                delimiter = ",";
            }

            var par = Parameters[i];
            if (par.DefaultValue == null)
            {
                result.Append($"{delimiter}{par.Name}: {par.Type}");
            }
            else
            {
                result.Append($"[{delimiter}{par.Name}: {par.Type}]");
            }
        }
        
        result.Append(')');
        return result.ToString();
    }
}