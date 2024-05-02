using System.Collections.Immutable;
using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Syntax;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Interpreter.Stdlib;

public class CompileTimeNameTable
{
    public record MemberVariable(string Name, BailaType Type, bool Immutable = false);
    public record MemberFunction(string Name, List<FunctionOverload> Overloads);

    public class Scope(Scope? parentScope = null)
    {
        private readonly Dictionary<string, MemberVariable> _variables = new();
        private readonly Dictionary<string, MemberFunction> _functions = new();

        public Scope? ParentScope { get; } = parentScope;

        public void AddVariable(string name, BailaType type)
        {
            _variables[name] = new MemberVariable(name, type, Immutable: false);
        }

        public void AddConstant(string name, BailaType type)
        {
            _variables[name] = new MemberVariable(name, type, Immutable: true);
        }

        public bool TryAddFunction(string name, FunctionOverload overload, out FunctionOverload? conflictingOverload)
        {
            MemberFunction function;

            if (Exists(name))
            {
                function = GetFunction(name)!;

                if (FunctionValue.HasConflictingOverload(overload, function.Overloads, out conflictingOverload))
                {
                    return false;
                }

                function.Overloads.Add(overload);
                return true;
            }
            
            function = new MemberFunction(name, []);
            function.Overloads.Add(overload);
            _functions[name] = function;
            conflictingOverload = null;
            return true;
        }

        public MemberVariable? GetVariable(string name)
        {
            return _variables.GetValueOrDefault(name);
        }

        public MemberFunction? GetFunction(string name)
        {
            return _functions.GetValueOrDefault(name);
        }

        public bool Exists(string name)
        {
            return ExistsVariable(name) || ExistsFunction(name);
        }

        public bool ExistsVariable(string name)
        {
            return _variables.ContainsKey(name);
        }

        public bool ExistsFunction(string name)
        {
            return _functions.ContainsKey(name);
        }

        public IEnumerable<MemberVariable> GetDefinedMembers()
        {
            return _variables.Values.ToImmutableList();
        }
    }

    private record ScopeFindData(bool IsFound = false, Scope? Scope = null)
    {
        public bool IsFound { get; internal set; } = IsFound;
        public Scope? Scope { get; internal set; } = Scope;
    }

    public static Scope CurrentScope { get; internal set; } = new();

    static CompileTimeNameTable()
    {
        CurrentScope.AddVariable("print", BailaType.Function);
        CurrentScope.AddVariable("input", BailaType.Function);
        CurrentScope.AddVariable("sum", BailaType.Function);
        CurrentScope.AddVariable("type_of", BailaType.Function);
    }

    public static MemberVariable? Get(string name)
    {
        var scope = FindScope(name);
        return scope is { IsFound: true, Scope: not null }
            ? scope.Scope.GetVariable(name)
            : null;
    }

    public static MemberFunction? GetFunction(string name)
    {
        var scope = FindScopeWithFunction(name);
        return scope is { IsFound: true, Scope: not null }
            ? scope.Scope.GetFunction(name)
            : null;
    }

    public static void AddVariable(string name, BailaType type)
    {
        CurrentScope.AddVariable(name, type);
    }

    public static void AddConstant(string name, BailaType type)
    {
        CurrentScope.AddConstant(name, type);
    }

    public static bool TryAddFunction(string name, FunctionOverload overload, out FunctionOverload? conflictingOverload)
    {
        return CurrentScope.TryAddFunction(name, overload, out conflictingOverload);
    }

    public static bool Exists(string name)
    {
        return FindScope(name).IsFound;
    }

    public static bool ExistsVariable(string name)
    {
        return FindScopeWithVariable(name).IsFound;
    }

    public static bool ExistsFunction(string name)
    {
        return FindScopeWithFunction(name).IsFound;
    }

    public static void PushScope()
    {
        CurrentScope = new Scope(CurrentScope);
    }

    public static void PopScope()
    {
        var parent = CurrentScope.ParentScope;
        if (parent != null)
        {
            CurrentScope = parent;
        }
    }

    private static ScopeFindData FindScope(string name)
    {
        var result = new ScopeFindData();

        var current = CurrentScope;
        do
        {
            if (current.Exists(name) != false)
            {
                result.IsFound = true;
                result.Scope = current;
                return result;
            }

            current = current.ParentScope;
        } while (current != null);

        result.IsFound = false;
        result.Scope = CurrentScope;
        return result;
    }

    private static ScopeFindData FindScopeWithVariable(string name)
    {
        var result = new ScopeFindData();

        var current = CurrentScope;
        do
        {
            if (current.ExistsVariable(name))
            {
                result.IsFound = true;
                result.Scope = current;
                return result;
            }

            current = current.ParentScope;
        } while (current != null);

        result.IsFound = false;
        result.Scope = CurrentScope;
        return result;
    }

    private static ScopeFindData FindScopeWithFunction(string name)
    {
        var result = new ScopeFindData();

        var current = CurrentScope;
        do
        {
            if (current.ExistsFunction(name))
            {
                result.IsFound = true;
                result.Scope = current;
                return result;
            }

            current = current.ParentScope;
        } while (current != null);

        result.IsFound = false;
        result.Scope = CurrentScope;
        return result;
    }

    public static IEnumerable<MemberVariable> GetAllMembers()
    {
        var result = new List<MemberVariable>();

        var current = CurrentScope;
        do
        {
            foreach (var variable in CurrentScope.GetDefinedMembers())
            {
                if (result.Any(m => m.Name == variable.Name)) continue;
                result.Add(variable);
            }

            current = current.ParentScope;
        } while (current != null);

        return result;
    }
}