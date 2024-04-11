using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Functional;

public record FunctionParameter(string name, BailaType type, IExpression? defaultValue = null, bool vararg = false);