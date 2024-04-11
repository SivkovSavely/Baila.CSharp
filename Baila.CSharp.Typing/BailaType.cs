﻿namespace Baila.CSharp.Typing;

public record BailaType(string ClassName, bool Nullable = false, List<BailaType>? Generics = null)
{
    public static readonly BailaType Bool = new("Bool");
    public static readonly BailaType Float = new("Float");
    public static readonly BailaType Int = new("Int");
    public static readonly BailaType String = new("String");
    public static readonly BailaType Function = new("Function");
}