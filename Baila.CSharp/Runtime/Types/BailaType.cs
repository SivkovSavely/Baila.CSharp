namespace Baila.CSharp.Typing;

public record BailaType(string ClassName, bool Nullable = false, List<BailaType>? Generics = null)
{
    public static readonly BailaType Any = new("Any");
    public static readonly BailaType Bool = new("Bool");
    public static readonly BailaType Float = new("Float");
    public static readonly BailaType Int = new("Int");
    public static readonly BailaType String = new("String");
    public static readonly BailaType Function = new("Function");

    public bool IsImplicitlyConvertibleTo(BailaType targetType)
    {
        if (targetType == Any)
        {
            // Anything is implicitly convertible to Any
            return true;
        }

        if (targetType == this)
        {
            // The type is implicitly convertible to itself
            return true;
        }

        return false;
    }

    public bool IsExplicitlyConvertibleTo(BailaType targetType)
    {
        if (IsImplicitlyConvertibleTo(targetType))
        {
            // If something is implicitly convertible, then it's also explicitly convertible.
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return (Nullable ? "?" : "") +
               (Generics != null ? $"<{string.Join(", ", Generics.Select(x => x.ToString()))}>" : "") +
               ClassName;
    }
}