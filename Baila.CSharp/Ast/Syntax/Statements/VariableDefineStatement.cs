using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Lexer;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public record VariableDefineStatement(
    Token VarToken,
    Token NameIdentifier,
    BailaType? Type,
    SyntaxNodeSpan? TypeSpan,
    IExpression? ValueExpression) : IStatement
{
    public string Name { get; init; } = NameIdentifier.Value!;

    public SyntaxNodeSpan Span { get; init; } = ValueExpression != null
        ? SyntaxNodeSpan.Merge(VarToken, NameIdentifier, ValueExpression)
        : TypeSpan != null
            ? SyntaxNodeSpan.Merge(VarToken.Span, NameIdentifier.Span, TypeSpan.Value)
            : SyntaxNodeSpan.Merge(VarToken, NameIdentifier);

    public void Execute()
    {
        BailaType variableType, valueType;
        IValue value;
        if (Type == null)
        {
            // Infer from the value
            if (ValueExpression == null)
            {
                throw new Exception(
                    $"Error: either type or value should be provided for the variable {Name}");
            }
            
            value = ValueExpression.Evaluate();
            valueType = ValueExpression.GetBailaType()!;
            variableType = valueType;
        }
        else if (ValueExpression == null)
        {
            // Infer default value from the type
            value = Type.GetDefaultValue();
            valueType = value.GetBailaType();
            variableType = Type;
        }
        else
        {
            value = ValueExpression.Evaluate();
            valueType = ValueExpression.GetBailaType()!;
            variableType = Type;
        }
        
        if (!valueType.IsImplicitlyConvertibleTo(variableType))
        {
            throw new Exception($"Cannot convert '{valueType}' to '{Type}'");
        }
        
        NameTable.AddVariable(Name, variableType, value);
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitVariableDefineStatement(this);
    }

    public override string ToString()
    {
        return $"VariableDefineStatement(Name={Name}, " +
               $"Type={Type}, " +
               $"Value={ValueExpression})";
    }
}