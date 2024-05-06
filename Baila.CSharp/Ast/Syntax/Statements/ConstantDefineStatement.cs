using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public record ConstantDefineStatement(
    Token ConstKeyword,
    Token NameIdentifier,
    Token EqualsToken,
    IExpression Value) : IStatement
{
    public string Name { get; set; } = NameIdentifier.Value!;
    
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.Merge(ConstKeyword, NameIdentifier, EqualsToken, Value);
    public void Execute()
    {
        NameTable.AddConstant(Name, Value.Evaluate());
    }


    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitConstantDefineStatement(this);
    }

    public override string ToString()
    {
        return $"ConstantDefineStatement(Name={Name}, " +
               $"Value={Value})";
    }
}