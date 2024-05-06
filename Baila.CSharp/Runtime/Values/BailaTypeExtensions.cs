using Baila.CSharp.Runtime.Types;

namespace Baila.CSharp.Runtime.Values;

public static class BailaTypeExtensions
{
    private static readonly Dictionary<BailaType, Func<IValue>> DefaultValueFactory;
    
    static BailaTypeExtensions()
    {
        DefaultValueFactory = new Dictionary<BailaType, Func<IValue>>();
        DefaultValueFactory[BailaType.Int] = () => new IntValue(0);
        DefaultValueFactory[BailaType.Float] = () => new FloatValue(0);
        DefaultValueFactory[BailaType.Bool] = () => new BooleanValue(false);
        DefaultValueFactory[BailaType.String] = () => new StringValue("");
    }
    
    public static IValue GetDefaultValue(this BailaType type)
    {
        if (DefaultValueFactory.TryGetValue(type, out var valueFactory))
        {
            return valueFactory();
        }

        if (type.Nullable)
        {
            throw new NotImplementedException("TODO: implement null reference");
        }

        throw new InvalidOperationException($"No default value for non-nullable type {type}");
    }
}