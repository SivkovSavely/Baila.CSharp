using System.Collections.Immutable;
using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Interpreter.Stdlib;

public class NameTable
{
    public record Member(string Name, BailaType Type, IValue Value, bool Immutable = false)
    {
        public IValue Value { get; internal set; } = Value;
    }

    public class Scope(Scope? parentScope = null)
    {
        private readonly Dictionary<string, Member> _members = new();

        public Scope? ParentScope { get; } = parentScope;

        public void AddVariable(string name, BailaType type, IValue value)
        {
            if (Exists(name))
            {
                throw new Exception($"'{name}' is already defined");
            }

            if (!value.GetBailaType().IsImplicitlyConvertibleTo(type))
            {
                throw new Exception($"Cannot convert '{value.GetBailaType()}' to '{type}'");
            }

            _members[name] = new Member(name, type, value, Immutable: false);
        }

        public void AddVariableInferred(string name, IValue value)
        {
            AddVariable(name, value.GetBailaType(), value);
        }

        public void AddConstant(string name, IValue value)
        {
            if (Exists(name))
            {
                throw new Exception($"'{name}' is already defined");
            }

            if (!value.GetBailaType().IsImplicitlyConvertibleTo(value.GetBailaType()))
            {
                throw new Exception($"Cannot convert '{value.GetBailaType()}' to '{value.GetBailaType()}'");
            }

            _members[name] = new Member(name, value.GetBailaType(), value, Immutable: true);
        }

        public Member GetMember(string name)
        {
            if (_members.TryGetValue(name, out var member))
            {
                return member;
            }

            throw new Exception($"'{name}' is not defined");
        }

        public void SetVariable(string name, IValue value)
        {
            var member = GetMember(name);

            if (member.Immutable)
            {
                throw new Exception($"'{name}' is constant");
            }

            if (!value.GetBailaType().IsImplicitlyConvertibleTo(member.Type))
            {
                throw new Exception($"Cannot convert '{value.GetBailaType()}' to '{member.Type}'");
            }

            member.Value = value;
        }

        public bool Exists(string name)
        {
            return _members.ContainsKey(name);
        }

        public IEnumerable<Member> GetDefinedMembers()
        {
            return _members.Values.ToImmutableList();
        }
    }

    private record ScopeFindData(bool IsFound = false, Scope? Scope = null)
    {
        public bool IsFound { get; internal set; } = IsFound;
        public Scope? Scope { get; internal set; } = Scope;
    }

    // private NameTable()
    // {
    //     Instance ??= new();
    // }
    //
    // public static NameTable Instance { get; private set; } = null!;

    public static Scope CurrentScope { get; internal set; } = new();

    static NameTable()
    {
        CurrentScope.AddVariableInferred("print", FunctionValue.WithOverloads(
            overloads: [
                new FunctionOverload(
                    args => { Console.WriteLine(args.GetString(0)); },
                    [
                        new FunctionParameter("x", BailaType.Any)
                    ],
                    null),
            ],
            name: "print"));
        CurrentScope.AddVariableInferred("input", FunctionValue.WithOverloads(
            overloads: [
                new FunctionOverload(
                    args =>
                    {
                        var prompt = args.ArgsByIndex.FirstOrDefault()?.GetAsString() ?? "";
                        Console.Write(prompt);
                        return new StringValue(Console.ReadLine() ?? "");
                    },
                    [
                        new FunctionParameter("prompt", BailaType.String, new StringValueExpression(""))
                    ],
                    BailaType.String),
            ],
            name: "input"));
        CurrentScope.AddVariableInferred("sum", FunctionValue.WithOverloads(
            overloads:
            [
                new FunctionOverload(
                    args => new IntValue(args.GetInteger(0)),
                    [
                        new FunctionParameter("x1", BailaType.Int)
                    ],
                    null),
                new FunctionOverload(
                    args => new IntValue(args.GetInteger(0) + args.GetInteger(1)),
                    [
                        new FunctionParameter("x1", BailaType.Int),
                        new FunctionParameter("x2", BailaType.Int)
                    ],
                    null),
                new FunctionOverload(
                    args => new IntValue(args.GetInteger(0) + args.GetInteger(1) + args.GetInteger(2)),
                    [
                        new FunctionParameter("x1", BailaType.Int),
                        new FunctionParameter("x2", BailaType.Int),
                        new FunctionParameter("x3", BailaType.Int)
                    ],
                    null),
            ],
            name: "sum"));
        CurrentScope.AddVariableInferred("type_of", FunctionValue.WithOverloads(
            overloads:
            [
                new FunctionOverload(
                    args => new StringValue(args.Get(0).GetBailaType().ToString()),
                    [
                        new FunctionParameter("x", BailaType.Any)
                    ],
                    null),
            ],
            name: "type_of"));
    }

    public static Member Get(string name)
    {
        var scope = FindScope(name);
        if (scope is { IsFound: true, Scope: not null })
        {
            return scope.Scope.GetMember(name);
        }

        throw new Exception($"ReferenceError: '{name}' is not defined");
    }

    public static void AddVariable(string name, BailaType type, IValue value)
    {
        CurrentScope.AddVariable(name, type, value);
        CompileTimeNameTable.AddVariable(name, type);
    }

    public static void AddVariableInferred(string name, IValue value)
    {
        CurrentScope.AddVariableInferred(name, value);
        CompileTimeNameTable.AddVariable(name, value.GetBailaType());
    }

    public static void AddConstant(string name, IValue value)
    {
        CurrentScope.AddConstant(name, value);
        CompileTimeNameTable.AddConstant(name, value.GetBailaType());
    }

    public static void Set(string name, IValue value)
    {
        var scope = FindScope(name);
        if (scope is { IsFound: true, Scope: not null })
        {
            scope.Scope.SetVariable(name, value);
            return;
        }

        throw new Exception($"ReferenceError: '{name}' is not defined");
    }

    public static bool Exists(string name)
    {
        return FindScope(name).IsFound;
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

        Scope? current = CurrentScope;
        do
        {
            if (current?.Exists(name) != false)
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

    public static IEnumerable<Member> GetAllMembers()
    {
        var result = new List<Member>();

        Scope? current = CurrentScope;
        
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