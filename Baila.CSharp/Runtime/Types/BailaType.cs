namespace Baila.CSharp.Typing;

public record BailaType(string ClassName, bool Nullable = false, List<BailaType>? Generics = null)
{
    public static readonly BailaType Any = new("Any");
    public static readonly BailaType Number = new("Number");
    public static readonly BailaType Float = new("Float", baseType: Number);
    public static readonly BailaType Int = new("Int", baseType: Number);
    public static readonly BailaType Bool = new("Bool");
    public static readonly BailaType String = new("String");
    public static readonly BailaType Function = new("Function");

    /// <summary>
    /// Base type of this type. It always has a value (Any if not specified in the constructor), with the exception
    /// of Any itself, which has BaseType = null.
    /// </summary>
    public BailaType? BaseType { get; } = Any;

    public BailaType(string ClassName, BailaType baseType, bool Nullable = false, List<BailaType>? Generics = null)
        : this(ClassName, Nullable, Generics)
    {
        BaseType = baseType;
    }

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

        if (targetType.ClassName == ClassName && targetType.Nullable && !Nullable)
        {
            // We can implicitly convert non-nullable T to nullable T.
            return true;
        }

        if (targetType == BaseType)
        {
            // If ThisType extends TargetType, we can convert ThisType to TargetType implicitly.
            // Example:
            // class Animal {}
            // class Cat : Animal {}
            // var a: Animal = Cat() # Cat can be implicitly converted to Animal,
            //   therefore Cat.IsImplicitlyConvertibleTo(Animal) should be true.
            return true;
        }

        if (BaseType != null && BaseType.IsImplicitlyConvertibleTo(targetType))
        {
            // Previous rule, but recursive to many layers down.
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