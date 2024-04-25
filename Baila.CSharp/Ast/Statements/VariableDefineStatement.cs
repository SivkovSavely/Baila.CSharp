using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public class VariableDefineStatement(string name, BailaType? type, IExpression? valueExpression) : IStatement
{
    public string Name { get; } = name;
    public BailaType? Type { get; } = type;
    public IExpression? ValueExpression { get; } = valueExpression;

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