using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Statements;

public class VariableDefineStatement(string name, BailaType? type, IExpression? value) : IStatement
{
    public string Name { get; } = name;
    public BailaType? Type { get; } = type;
    public IExpression? Value { get; } = value;

    public void Execute()
    {
        if (Type == null)
        {
            // Infer from the value
            if (Value == null)
            {
                throw new Exception(
                    $"Error: either type or value should be provided for the variable {Name}");
            }

            var evaled = Value.Evaluate();
            NameTable.AddVariable(Name, evaled.GetBailaType(), evaled);
            return;
        }

        if (Value == null)
        {
            // Infer default value from the type
            NameTable.AddVariable(Name, Type, Type.GetDefaultValue());
            return;
        }
        
        NameTable.AddVariable(Name, Type, Value.Evaluate());
    }

    public override string ToString()
    {
        return $"VariableDefineStatement(Name={Name}, " +
               $"Type={Type}, " +
               $"Value={Value})";
    }
}