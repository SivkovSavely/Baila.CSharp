using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Tests.Infrastructure;
using FluentAssertions;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class BailaTypeTests : TestsBase
{
    public BailaTypeTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Theory]
    [MemberData(nameof(GetBuiltinBailaTypes))]
    public void Any_IsASupertype_ForAllDefinedTypes(BailaType type)
    {
        type.IsImplicitlyConvertibleTo(BailaType.Any).Should().BeTrue();
    }

    [Theory]
    [InlineData(nameof(BailaType.Int))]
    [InlineData(nameof(BailaType.Float))]
    public void Number_IsASupertype_ForNumericTypes(string typeName)
    {
        GetBailaTypeByName(typeName).IsImplicitlyConvertibleTo(BailaType.Number).Should().BeTrue();
    }

    [Fact]
    public void IsImplicitlyConvertibleTo_SupportsArbitraryNestingOfBaseTypes()
    {
        var globalParent = new BailaType("GlobalParent"); // GlobalParent < Any
        var child1 = new BailaType("Child1", baseType: globalParent); // Child1 < GlobalParent < Any
        var child2 = new BailaType("Child2", baseType: globalParent); // Child2 < GlobalParent < Any
        var child11 = new BailaType("Child11", baseType: child1); // Child11 < Child1 < GlobalParent < Any
        var child12 = new BailaType("Child12", baseType: child1); // Child12 < Child1 < GlobalParent < Any
        var child21 = new BailaType("Child21", baseType: child2); // Child21 < Child2 < GlobalParent < Any
        var child22 = new BailaType("Child22", baseType: child2); // Child22 < Child2 < GlobalParent < Any

        // Happy paths
        globalParent.IsImplicitlyConvertibleTo(BailaType.Any).Should().BeTrue();
        child11.IsImplicitlyConvertibleTo(child1).Should().BeTrue();
        child12.IsImplicitlyConvertibleTo(child1).Should().BeTrue();
        child21.IsImplicitlyConvertibleTo(child2).Should().BeTrue();
        child22.IsImplicitlyConvertibleTo(child2).Should().BeTrue();
        child1.IsImplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child2.IsImplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child11.IsImplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child12.IsImplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child21.IsImplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child22.IsImplicitlyConvertibleTo(globalParent).Should().BeTrue();
        
        // Negative testing
        child11.IsImplicitlyConvertibleTo(child2).Should().BeFalse();
        child12.IsImplicitlyConvertibleTo(child2).Should().BeFalse();
        child21.IsImplicitlyConvertibleTo(child1).Should().BeFalse();
        child22.IsImplicitlyConvertibleTo(child1).Should().BeFalse();
        globalParent.IsImplicitlyConvertibleTo(child1).Should().BeFalse();
        globalParent.IsImplicitlyConvertibleTo(child2).Should().BeFalse();
        globalParent.IsImplicitlyConvertibleTo(child11).Should().BeFalse();
        globalParent.IsImplicitlyConvertibleTo(child12).Should().BeFalse();
        globalParent.IsImplicitlyConvertibleTo(child21).Should().BeFalse();
        globalParent.IsImplicitlyConvertibleTo(child22).Should().BeFalse();
        BailaType.Any.IsImplicitlyConvertibleTo(globalParent).Should().BeFalse();
        BailaType.Any.IsImplicitlyConvertibleTo(child1).Should().BeFalse();
        BailaType.Any.IsImplicitlyConvertibleTo(child2).Should().BeFalse();
        BailaType.Any.IsImplicitlyConvertibleTo(child11).Should().BeFalse();
        BailaType.Any.IsImplicitlyConvertibleTo(child12).Should().BeFalse();
        BailaType.Any.IsImplicitlyConvertibleTo(child21).Should().BeFalse();
        BailaType.Any.IsImplicitlyConvertibleTo(child22).Should().BeFalse();
    }

    [Fact(Skip = "IsExplicitlyConvertibleTo is currently not implemented until the end")]
    public void IsExplicitlyConvertibleTo_SupportsArbitraryNestingOfBaseTypes()
    {
        var globalParent = new BailaType("GlobalParent"); // GlobalParent < Any
        var child1 = new BailaType("Child1", baseType: globalParent); // Child1 < GlobalParent < Any
        var child2 = new BailaType("Child2", baseType: globalParent); // Child2 < GlobalParent < Any
        var child11 = new BailaType("Child11", baseType: child1); // Child11 < Child1 < GlobalParent < Any
        var child12 = new BailaType("Child12", baseType: child1); // Child12 < Child1 < GlobalParent < Any
        var child21 = new BailaType("Child21", baseType: child2); // Child21 < Child2 < GlobalParent < Any
        var child22 = new BailaType("Child22", baseType: child2); // Child22 < Child2 < GlobalParent < Any

        // Happy paths that are also implicitly convertible (any implicit conversion is also an explicit conversion)
        globalParent.IsExplicitlyConvertibleTo(BailaType.Any).Should().BeTrue();
        globalParent.IsExplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child1.IsExplicitlyConvertibleTo(child1).Should().BeTrue();
        child2.IsExplicitlyConvertibleTo(child2).Should().BeTrue();
        child11.IsExplicitlyConvertibleTo(child11).Should().BeTrue();
        child12.IsExplicitlyConvertibleTo(child12).Should().BeTrue();
        child21.IsExplicitlyConvertibleTo(child21).Should().BeTrue();
        child22.IsExplicitlyConvertibleTo(child22).Should().BeTrue();
        child11.IsExplicitlyConvertibleTo(child1).Should().BeTrue();
        child12.IsExplicitlyConvertibleTo(child1).Should().BeTrue();
        child21.IsExplicitlyConvertibleTo(child2).Should().BeTrue();
        child22.IsExplicitlyConvertibleTo(child2).Should().BeTrue();
        child1.IsExplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child2.IsExplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child11.IsExplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child12.IsExplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child21.IsExplicitlyConvertibleTo(globalParent).Should().BeTrue();
        child22.IsExplicitlyConvertibleTo(globalParent).Should().BeTrue();

        // Happy paths that are not implicitly convertible
        BailaType.Any.IsExplicitlyConvertibleTo(globalParent).Should().BeTrue();
        BailaType.Any.IsExplicitlyConvertibleTo(child1).Should().BeTrue();
        BailaType.Any.IsExplicitlyConvertibleTo(child2).Should().BeTrue();
        BailaType.Any.IsExplicitlyConvertibleTo(child11).Should().BeTrue();
        BailaType.Any.IsExplicitlyConvertibleTo(child12).Should().BeTrue();
        BailaType.Any.IsExplicitlyConvertibleTo(child21).Should().BeTrue();
        BailaType.Any.IsExplicitlyConvertibleTo(child22).Should().BeTrue();
        globalParent.IsExplicitlyConvertibleTo(child1).Should().BeTrue();
        globalParent.IsExplicitlyConvertibleTo(child2).Should().BeTrue();
        globalParent.IsExplicitlyConvertibleTo(child11).Should().BeTrue();
        globalParent.IsExplicitlyConvertibleTo(child12).Should().BeTrue();
        globalParent.IsExplicitlyConvertibleTo(child21).Should().BeTrue();
        globalParent.IsExplicitlyConvertibleTo(child22).Should().BeTrue();
        child1.IsExplicitlyConvertibleTo(child11).Should().BeTrue();
        child1.IsExplicitlyConvertibleTo(child12).Should().BeTrue();
        child2.IsExplicitlyConvertibleTo(child21).Should().BeTrue();
        child2.IsExplicitlyConvertibleTo(child22).Should().BeTrue();
        
        // Negative testing
        child1.IsExplicitlyConvertibleTo(child2).Should().BeFalse();
        child2.IsExplicitlyConvertibleTo(child1).Should().BeFalse();
        child11.IsExplicitlyConvertibleTo(child21).Should().BeFalse();
        child11.IsExplicitlyConvertibleTo(child22).Should().BeFalse();
        child12.IsExplicitlyConvertibleTo(child21).Should().BeFalse();
        child12.IsExplicitlyConvertibleTo(child22).Should().BeFalse();
        child21.IsExplicitlyConvertibleTo(child11).Should().BeFalse();
        child21.IsExplicitlyConvertibleTo(child12).Should().BeFalse();
        child22.IsExplicitlyConvertibleTo(child11).Should().BeFalse();
        child22.IsExplicitlyConvertibleTo(child12).Should().BeFalse();
    }

    [Fact]
    public void BailaType_GetHashCode_EqualsForEqualTypes()
    {
        var t11 = new BailaType("T1");
        var t12 = new BailaType("T1");
        t12.GetHashCode().Should().Be(t11.GetHashCode());

        var t21 = new BailaType("T2", Generics: []);
        var t22 = new BailaType("T2", Generics: []);
        t22.GetHashCode().Should().Be(t21.GetHashCode());

        var t31 = new BailaType("T3", Generics: [t11]);
        var t32 = new BailaType("T3", Generics: [t12]);
        t32.GetHashCode().Should().Be(t31.GetHashCode());
    }

    [Fact]
    public void BailaType_EqualTypes_Equal()
    {
        var t11 = new BailaType("T1");
        var t12 = new BailaType("T1");
        (t12 == t11).Should().BeTrue();
        t12.Equals(t11).Should().BeTrue();

        var t21 = new BailaType("T2", Generics: []);
        var t22 = new BailaType("T2", Generics: []);
        (t22 == t21).Should().BeTrue();
        t22.Equals(t21).Should().BeTrue();

        var t31 = new BailaType("T3", Generics: [t11]);
        var t32 = new BailaType("T3", Generics: [t12]);
        (t32 == t31).Should().BeTrue();
        t32.Equals(t31).Should().BeTrue();
    }

    public static IEnumerable<object?[]> GetBuiltinBailaTypes()
    {
        return BuiltInBailaTypes.Select(f => new[]
        {
            f.GetValue(null)
        });
    }
}