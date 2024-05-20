using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Ast.Syntax;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Ast.Syntax.Statements;
using Baila.CSharp.Runtime.Types;

namespace Baila.CSharp.Ast.Diagnostics;

public class ParserDiagnostics
{
    public static ParserDiagnostic BP0000_Test(ISyntaxNode node, string[] relevantSourceLines) =>
        new(
            "BP0000",
            $"test diagnostic for {node.GetType().Name}",
            node,
            relevantSourceLines);
    public static ParserDiagnostic BP0001_UnexpectedToken(Token unexpectedToken, TokenType[] expectedTokenTypes, string[] relevantSourceLines) =>
        new(
            "BP0001",
            $"unexpected token '{unexpectedToken}', expected tokens: {string.Join(", ", expectedTokenTypes.Select(x => $"'{x}'"))}",
            unexpectedToken,
            relevantSourceLines);

    public static ParserDiagnostic BP0001_UnexpectedToken(Token unexpectedToken, string expected, string[] relevantSourceLines) =>
        new(
            "BP0001",
            $"unexpected token '{unexpectedToken}', expected {expected}",
            unexpectedToken,
            relevantSourceLines);
    public static ParserDiagnostic BP0002_ExpectedToInForLoop(Token unexpectedToken, string[] relevantSourceLines) =>
        new(
            "BP0002",
            "'to' expected in 'for' loop. For C-like style of 'for' loop, please use 'while' instead",
            unexpectedToken,
            relevantSourceLines);
    public static ParserDiagnostic BP0003_CountNotInferTypeOfNumber(Token malformedNumber, string[] relevantSourceLines) =>
        new(
            "BP0003",
            "could not infer type of the number",
            malformedNumber,
            relevantSourceLines);
    public static ParserDiagnostic BP0004_CannotRedefineVariable(VariableDefineStatement statement, string[] relevantSourceLines) =>
        new(
            "BP0004",
            $"cannot redefine variable '{statement.Name}'",
            statement,
            relevantSourceLines);
    public static ParserDiagnostic BP0005_CannotInferTypeOfVariable(VariableDefineStatement statement, string[] relevantSourceLines) =>
        new(
            "BP0005",
            $"cannot infer type of an implicitly typed variable '{statement.Name}'",
            statement,
            relevantSourceLines);
    public static ParserDiagnostic BP0006_CannotRedefineConstant(ConstantDefineStatement statement, string[] relevantSourceLines) =>
        new(
            "BP0006",
            $"cannot redefine constant '{statement.Name}'",
            statement,
            relevantSourceLines);
    public static ParserDiagnostic BP0007_OverloadExists(FunctionDefineStatement statement, FunctionOverload overload, FunctionOverload conflictingOverload, string[] relevantSourceLines) =>
        new(
            "BP0007",
            $"overload {overload} conflicts with overload {conflictingOverload}",
            statement,
            relevantSourceLines);
    public static ParserDiagnostic BP0008_VariableIsNotAFunction(string name, FunctionDefineStatement statement, string[] relevantSourceLines) =>
        new(
            "BP0008",
            $"cannot define function '{name}': a variable with this name is already defined",
            statement,
            relevantSourceLines);
    public static ParserDiagnostic BP0009_SymbolIsNotDefined(string name, ISyntaxNode syntaxNode, string[] relevantSourceLines) =>
        new(
            "BP0009",
            $"'{name}' is not defined",
            syntaxNode,
            relevantSourceLines);
    public static ParserDiagnostic BP0010_SymbolIsNotDefined(IExpression expression, string[] relevantSourceLines) =>
        new(
            "BP0010",
            "expression is not callable",
            expression,
            relevantSourceLines);
    public static ParserDiagnostic BP0011_CannotConvertTypeFromAToB(IExpression expression, BailaType fromType, BailaType toType, string[] relevantSourceLines) =>
        new(
            "BP0011",
            $"cannot convert from '{fromType}' to '{toType}'",
            expression,
            relevantSourceLines);
    public static ParserDiagnostic BP0012_HaveToAssignValueToAnyVariable(string name, VariableDefineStatement statement, string[] relevantSourceLines) =>
        new(
            "BP0012",
            $"variable '{name}' has to be assigned value because it has type 'Any'",
            statement,
            relevantSourceLines);
    public static ParserDiagnostic BP0013_RequiredParametersShouldBeBeforeOptionalParameters(string functionName, string reqParam, string optParam, ISyntaxNode node, string[] relevantSourceLines) =>
        new(
            "BP0013",
            $"in function '{functionName}', required parameter '{reqParam}' cannot be after an optional parameter '{optParam}'",
            node,
            relevantSourceLines);
    public static ParserDiagnostic BP0014_BinaryOperatorCannotBeUsedOnTypes(string opSymbol, BailaType leftType, BailaType rightType, IExpression node, string[] relevantSourceLines) =>
        new(
            "BP0014",
            $"cannot use the operator '{opSymbol}' on operands of types '{leftType}' and '{rightType}'",
            node,
            relevantSourceLines);
    public static ParserDiagnostic BP0015_UnaryOperatorCannotBeUsedOnType(string opSymbol, BailaType operandType, IExpression node, string[] relevantSourceLines) =>
        new(
            "BP0015",
            $"cannot use the operator '{opSymbol}' on an operand of type '{operandType}'",
            node,
            relevantSourceLines);
}

public class ParserDiagnostic : IDiagnostic
{
    private readonly string[] _relevantSourceLines;
    public string Code { get; }
    public string Message { get; }
    public ISyntaxNode SyntaxNode { get; }

    internal ParserDiagnostic(string code, string message, ISyntaxNode syntaxNode, string[] relevantSourceLines)
    {
        _relevantSourceLines = relevantSourceLines;
        Code = code;
        Message = message;
        SyntaxNode = syntaxNode;
    }

    public string GetCode()
    {
        return Code;
    }

    public string GetErrorMessage()
    {
        return Message;
    }

    public string GetFilename()
    {
        return SyntaxNode.Span.Filename;
    }

    public IEnumerable<DiagnosticLineSpan> GetLines()
    {
        for (int lineNumber = 0; lineNumber < SyntaxNode.Span.LineCount; lineNumber++)
        {
            var fullLine = _relevantSourceLines[lineNumber];
            int startColumn, length;
            
            if (SyntaxNode.Span.LineCount == 1) {
                startColumn = SyntaxNode.Span.StartColumn;
                length = SyntaxNode.Span.SyntaxLength;
            } else {
                if (lineNumber + 1 == SyntaxNode.Span.StartLine) {
                    startColumn = SyntaxNode.Span.StartColumn;
                    length = fullLine.Length - startColumn;
                } else if (lineNumber + 1 == SyntaxNode.Span.EndLine) {
                    startColumn = 1;
                    length = SyntaxNode.Span.EndColumn - 1;
                } else {
                    startColumn = 1;
                    length = fullLine.Length;
                }
            }
            
            yield return new DiagnosticLineSpan(
                fullLine,
                SyntaxNode.Span.StartLine + lineNumber,
                startColumn,
                length);
        }
    }

    public bool ShouldShowCode()
    {
        return true;
    }
}