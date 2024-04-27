﻿// #define PARSER_TRACE_ENABLED

using System.Globalization;
using System.Text;
using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Ast.Syntax.Statements;
using Baila.CSharp.Lexer;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Parser;

public class Parser(List<Token> tokens, CancellationToken? cancellationToken = null)
{
    private int _position;
    private readonly int _length = tokens.Count;

    private readonly Stack<int> _rollbackPositions = new();

    public Statements BuildAst()
    {
        var result = new Statements();

        while (!Match(TokenType.EndOfFile))
        {
            result.AddStatement(Statement());
        }

        return result;
    }

    private IStatement Statement(bool requireEndOfStatement = true)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        IStatement stmt = null!;

        if (Match(TokenType.If))
        {
            Trace("IfElseStatement");
            stmt = IfElseStatement();
        }
        else if (Match(TokenType.For))
        {
            Trace("ForStatement");
            stmt = ForStatement();
        }
        else if (Match(TokenType.While))
        {
            Trace("WhileStatement");
            stmt = WhileStatement();
        }
        else if (Match(TokenType.Do))
        {
            Trace("DoWhileStatement");
            stmt = DoWhileStatement();
        }
        else if (Match(TokenType.Var))
        {
            Trace("VariableDefineStatement");
            var name = Consume(TokenType.Identifier).Value!;
            BailaType? type = null;
            IExpression? value = null;

            if (Match(TokenType.Colon))
            {
                type = Type();
            }

            if (Match(TokenType.Eq))
            {
                value = Expression();
            }

            stmt = new VariableDefineStatement(name, type, value);
        }
        else if (Match(TokenType.Const))
        {
            Trace("ConstantDefineStatement");
            var name = Consume(TokenType.Identifier).Value!;
            Consume(TokenType.Eq);
            var value = Expression();

            stmt = new ConstantDefineStatement(name, value);
        }
        else if (Match(TokenType.Function))
        {
            Trace("FunctionDefinition");
            stmt = FunctionDefinition();
        }
        else if (Match(TokenType.Return))
        {
            Trace("ReturnStatement");
            if (LookMatch(0, TokenType.EndOfFile) || LookMatch(0, TokenType.Semicolon) || LookMatch(0, TokenType.EndOfLine) || LookMatch(0, TokenType.RightCurly))
            {
                stmt = new ReturnStatement();
            }
            else
            {
                stmt = new ReturnStatement(Expression());
            }
        }
        else if (Match(TokenType.Break))
        {
            Trace("Break");
            // TODO break statement
            throw new NotImplementedException();
        }
        else if (Match(TokenType.Continue))
        {
            Trace("Continue");
            // TODO continue statement
            throw new NotImplementedException();
        }
        else if (Match(TokenType.Class))
        {
            Trace("Class");
            // TODO class declaration
            throw new NotImplementedException();
        }
        else if (Match(TokenType.EndOfLine) || Match(TokenType.Semicolon))
        {
            Trace("EOL or Semicolon");
            return new NoOpStatement();
        }
        else
        {
            Trace("ExpressionStatement");
            stmt = new ExpressionStatement(Expression());
        }
        
        if (requireEndOfStatement)
        {
            if (!LookMatch(0, TokenType.EndOfLine) &&
                !LookMatch(0, TokenType.Semicolon) &&
                !LookMatch(0, TokenType.EndOfFile) &&
                !LookMatch(0, TokenType.RightCurly) &&
                !LookMatch(0, TokenType.Else))
            {
                throw new Exception($"Syntax error: unexpected token {Get()}, expected a new line or a semicolon");
            }

            if (LookMatch(0, TokenType.EndOfLine)) Consume(TokenType.EndOfLine);
            else if (LookMatch(0, TokenType.Semicolon)) Consume(TokenType.Semicolon);
            else if (LookMatch(0, TokenType.EndOfFile)) Consume(TokenType.EndOfFile);
        }

        if (stmt == null)
        {
            throw new Exception("Stmt is null");
        }

        return stmt;
    }

    private FunctionDefineStatement FunctionDefinition()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var name = Consume(TokenType.Identifier).Value!;
        var parameters = new List<FunctionParameter>();
        BailaType? returnType = null;

        if (Match(TokenType.LeftParen))
        {
            // TODO varargs
            while (!Match(TokenType.RightParen))
            {
                var paramName = Consume(TokenType.Identifier).Value!;
                Consume(TokenType.Colon);
                var paramType = Type();
                IExpression? defaultValue = null;
                if (Match(TokenType.Eq))
                {
                    defaultValue = Expression();
                }

                parameters.Add(
                    new FunctionParameter(
                        paramName, paramType, defaultValue, false));

                if (Match(TokenType.RightParen)) break;

                Consume(TokenType.Comma);
            }
        }

        var shouldTraverseBody = false;
        SkipOptionalNewline();
        if (Match(TokenType.Colon)) {
            // TODO redo to catch fully optional expression
            SkipOptionalNewline();
            returnType = Type();
        } else {
            // Return type is not specified, try to parse the return type from the body
            // If there are no return statements then the function is void.
            // ExprStmt are not allowed, we don't want func f: Number { g() } to return g()'s result.

            shouldTraverseBody = true;

            // TODO traverse function statements and retrieve all return statements, if any
        }
        SkipOptionalNewline();

        var body = StatementBlock(); // todo support '=>'

        if (shouldTraverseBody)
        {
            var returnStatements = ReturnStatementSearchingStatementTraverser.Search(body);
            if (returnStatements.Count == 0)
            {
                returnType = null; // void
            }
            else if (returnStatements.Count == 1)
            {
                returnType = returnStatements.First().ReturnExpression?.GetBailaType(); // type of return if it has an expression or void if return doesn't have an expression
            }
            else
            {
                throw new NotImplementedException("Inferring the return type of multiple return statements is not implemented yet");
            }
        }

        return new FunctionDefineStatement(
            name, parameters, body, returnType);
    }

    private BailaType Type()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        // nullable check
        var isNullable = Match(TokenType.Question);
        
        // generics
        List<BailaType>? genericsList = null;
        if (Match(TokenType.Lt)) // TODO support LtLt e.g. <<Int>List>List
        {
            genericsList = new();
            while (!Match(TokenType.Gt))
            {
                var genericType = Type();
                genericsList.Add(genericType);

                if (Match(TokenType.Gt)) break;

                Consume(TokenType.Comma);
            }
        }
        
        // type name
        var result = new BailaType(Consume(TokenType.Identifier).Value!, isNullable, genericsList);

        return result;
    }

    private IStatement IfElseStatement()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var condition = Expression();
        var trueStmt = StatementOrBlock(false);

        _rollbackPositions.Push(_position);
        SkipConsecutive(TokenType.EndOfLine);
        
        IStatement? falseStmt = null;
        if (Match(TokenType.Else))
        {
            falseStmt = StatementOrBlock(false);
            _rollbackPositions.Pop();
        }
        else
        {
            _position = _rollbackPositions.Pop();
        }

        return new IfElseStatement(condition, trueStmt, falseStmt);
    }

    private IStatement ForStatement()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var optionalLeftParen = Match(TokenType.LeftParen);

        var counterVariable = Consume(TokenType.Identifier).Value!;
        Consume(TokenType.Eq);
        var initialValue = Expression();

        if (!(Get().Type == TokenType.Identifier && Get().Value == "to"))
        {
            throw new Exception(
                "Syntax error: 'to' expected in 'for' loop. For C-like style of 'for' loop, please use 'while' instead");
        }

        Match(TokenType.Identifier);

        var finalValue = Expression();

        IExpression stepValue;
        if (Get().Type == TokenType.Identifier && Get().Value == "step")
        {
            Match(TokenType.Identifier);
            stepValue = Expression();
        }
        else
        {
            stepValue = new IntValueExpression(1);            
        }

        if (optionalLeftParen)
        {
            Match(TokenType.RightParen);
        }

        var body = StatementOrBlock();

        return new ForStatement(counterVariable, initialValue, finalValue, stepValue, body);
    }

    private IStatement WhileStatement()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var condition = Expression();
        var body = StatementOrBlock();

        return new WhileStatement(condition, body);
    }

    private IStatement DoWhileStatement()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var body = StatementOrBlock();
        Consume(TokenType.While);
        var condition = Expression();

        return new DoWhileStatement(condition, body);
    }

    private IStatement StatementOrBlock(bool requireEndOfStatement = true)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        SkipConsecutive(TokenType.EndOfLine);

        if (LookMatch(0, TokenType.LeftCurly))
        {
            return StatementBlock(requireEndOfStatement);
        }

        return Statement(requireEndOfStatement);
    }

    private IStatement StatementBlock(bool requireEndOfStatement = true)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var block = new BlockStatement();
        Consume(TokenType.LeftCurly);
        while (!Match(TokenType.RightCurly))
        {
            block.AddStatement(Statement(requireEndOfStatement));
        }

        return block;
    }

    private Token Consume(TokenType tokenType)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var current = Get();
        if (current.Type != tokenType)
        {
            throw new Exception($"Syntax error: unexpected token {current.Type}, expected {tokenType}");
        }

        _position++;
        return current;
    }

    private void SkipConsecutive(TokenType tokenType)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        while (Get().Type == tokenType)
        {
            Consume(tokenType);
        }
    }

    private bool LookMatch(int relative, TokenType tokenType)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        return Get(relative).Type == tokenType;
    }

    private IExpression Expression()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        Trace("Expression");
        return Assignment();
    }

    private IExpression Assignment()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        if (LookMatch(0, TokenType.Identifier) && LookMatch(1, TokenType.Eq))
        {
            var name = Consume(TokenType.Identifier).Value!;
            Consume(TokenType.Eq);
            var expr = Assignment();

            return new AssignmentExpression(name, expr);
        }

        // TODO +-, -=, etc

        return BitwiseOr();
    }

    private IExpression BitwiseOr()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var result = BitwiseXor();

        while (true)
        {
            if (Match(TokenType.Bar))
            {
                result = new BinaryExpression(BinaryExpression.Operation.BitwiseOr, result, BitwiseXor());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression BitwiseXor()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var result = BitwiseAnd();

        while (true)
        {
            if (Match(TokenType.Caret))
            {
                result = new BinaryExpression(BinaryExpression.Operation.BitwiseXor, result, BitwiseAnd());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression BitwiseAnd()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var result = LogicalOr();

        while (true)
        {
            if (Match(TokenType.Amp))
            {
                result = new BinaryExpression(BinaryExpression.Operation.BitwiseAnd, result, LogicalOr());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression LogicalOr()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var result = LogicalAnd();

        while (true)
        {
            if (Match(TokenType.BarBar))
            {
                result = new BinaryExpression(BinaryExpression.Operation.LogicalOr, result, LogicalAnd());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression LogicalAnd()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var result = Equality();

        while (true)
        {
            if (Match(TokenType.AmpAmp))
            {
                result = new BinaryExpression(BinaryExpression.Operation.LogicalAnd, result, Equality());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression Equality()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var result = NumberRelation();

        while (true)
        {
            if (Match(TokenType.EqEq))
            {
                result = new BinaryExpression(BinaryExpression.Operation.Equality, result, NumberRelation());
                continue;
            }

            if (Match(TokenType.ExclEq))
            {
                result = new BinaryExpression(BinaryExpression.Operation.Inequality, result, NumberRelation());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression NumberRelation()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var result = Addition();

        while (true)
        {
            if (Match(TokenType.Lt))
            {
                result = new BinaryExpression(BinaryExpression.Operation.LessThan, result, Addition());
                continue;
            }

            if (Match(TokenType.LtEq))
            {
                result = new BinaryExpression(BinaryExpression.Operation.LessThanOrEqual, result, Addition());
                continue;
            }

            if (Match(TokenType.Gt))
            {
                result = new BinaryExpression(BinaryExpression.Operation.GreaterThan, result, Addition());
                continue;
            }

            if (Match(TokenType.GtEq))
            {
                result = new BinaryExpression(BinaryExpression.Operation.GreaterThanOrEqual, result, Addition());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression Addition()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var result = Multiplication();

        while (true)
        {
            if (Match(TokenType.Plus))
            {
                result = new BinaryExpression(BinaryExpression.Operation.Addition, result, Multiplication());
                continue;
            }

            if (Match(TokenType.Minus))
            {
                result = new BinaryExpression(BinaryExpression.Operation.Subtraction, result, Multiplication());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression Multiplication()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var result = Power();

        while (true)
        {
            if (Match(TokenType.Slash))
            {
                result = new BinaryExpression(BinaryExpression.Operation.FloatDivision, result, Power());
                continue;
            }

            if (Match(TokenType.SlashSlash))
            {
                result = new BinaryExpression(BinaryExpression.Operation.IntegerDivision, result, Power());
                continue;
            }

            if (Match(TokenType.Star))
            {
                result = new BinaryExpression(BinaryExpression.Operation.Multiplication, result, Power());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression Power()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var result = Unary();

        while (true)
        {
            if (Match(TokenType.StarStar))
            {
                result = new BinaryExpression(BinaryExpression.Operation.Power, result, Unary());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression Unary()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        if (Match(TokenType.Tilde))
        {
            return new PrefixUnaryExpression(PrefixUnaryExpression.Operation.BitwiseNegation, Unary());
        }

        if (Match(TokenType.Excl))
        {
            return new PrefixUnaryExpression(PrefixUnaryExpression.Operation.LogicalNegation, Unary());
        }

        if (Match(TokenType.Plus))
        {
            return new PrefixUnaryExpression(PrefixUnaryExpression.Operation.Plus, Unary());
        }

        if (Match(TokenType.Minus))
        {
            return new PrefixUnaryExpression(PrefixUnaryExpression.Operation.Minus, Unary());
        }

        if (Match(TokenType.Typeof))
        {
            return new TypeOfExpression(Unary());
        }

        return Primary();
    }

    private IExpression Primary()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        Trace("Primary");
        var current = Get();
        IExpression? result = null;

        // (expr)
        if (Match(TokenType.LeftParen))
        {
            Trace("LeftParen");
            var expression = Expression();
            Consume(TokenType.RightParen);
            result = expression;
        }
        // [ listElem1, listElem2, ..., listElemN ]
        else if (Match(TokenType.LeftBracket))
        {
            Trace("LeftBracket");
            var expressionList = new List<IExpression>();

            while (!Match(TokenType.RightBracket))
            {
                expressionList.Add(Expression());
                Match(TokenType.Comma); // TODO require comma, use Consume instead of Match
            }

            throw new NotImplementedException("List expressions are not implemented yet");
        }
        // Numbers
        else if (Match(TokenType.NumberLiteral))
        {
            Trace("NumberLiteral");
            var currentNum = current.Value!;
            var suffix = currentNum.Last();
            var number = suffix is 'c' ? currentNum[..^1] : currentNum;

            result = suffix switch
            {
                'c' => throw new NotImplementedException("Char number literals are not implemented yet"),
                _ when long.TryParse(number, CultureInfo.InvariantCulture, out var intNumber) => new IntValueExpression(intNumber),
                _ when double.TryParse(number, CultureInfo.InvariantCulture, out var floatNumber) => new FloatValueExpression(floatNumber),
                _ => throw new Exception($"Could not infer type of the number: '{number}'")
            };
        }
        // Strings
        else if (Match(TokenType.StringLiteral))
        {
            Trace("StringLiteral");
            result = new StringValueExpression(current.Value!);
        }
        // Interpolated Strings
        else if (Match(TokenType.PrivateStringConcat))
        {
            Trace("PrivateStringConcat");
            var fixedStrings = new List<string>();
            var expressions = new List<IExpression>();
            bool? expressionWasLast = null;

            Consume(TokenType.PrivateStringConcatStart);

            while (!Match(TokenType.PrivateStringConcatEnd))
            {
                if (Get().Type == TokenType.StringLiteral)
                {
                    fixedStrings.Add(Consume(TokenType.StringLiteral).Value!);
                    expressionWasLast = false;
                }
                else
                {
                    if (expressionWasLast is null or true)
                    {
                        fixedStrings.Add(""); // We always have to have a fixed string first or between expressions
                    }
                    expressions.Add(Expression());
                    expressionWasLast = true;
                }

                if (Match(TokenType.PrivateStringConcatEnd)) break;

                Consume(TokenType.Comma);
            }
            
            result = new StringConcatExpression(fixedStrings, expressions);
        }
        // true and false
        else if (Match(TokenType.True))
        {
            Trace("True");
            result = new BoolValueExpression(true);
        }
        else if (Match(TokenType.False))
        {
            Trace("False");
            result = new BoolValueExpression(false);
        }
        // Variables and constants
        else if (Match(TokenType.Identifier))
        {
            Trace("Identifier");
            result = new VariableExpression(current.Value!);
        }

        if (result == null)
        {
            throw new Exception($"Syntax error: unexpected {current}");
        }

        cancellationToken?.ThrowIfCancellationRequested();

        if (LookMatch(0, TokenType.LeftParen)) // TODO do this in the infinite loop, alongside LeftBracket and Dot
        {
            Trace("LeftParen after");
            var args = new List<IExpression>();
            Consume(TokenType.LeftParen);

            while (!Match(TokenType.EndOfFile) && !Match(TokenType.RightParen))
            {
                args.Add(Expression());
                if (Match(TokenType.RightParen))
                {
                    break;
                }

                Consume(TokenType.Comma);
            }

            result = new FunctionCallExpression(result, args);
        }
        
        // TODO [array access], (function call) and object.dot.access
        // TODO expr = expr advanced assignment

        return result;
    }

    private bool Match(TokenType tokenType)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        if (Get().Type != tokenType)
        {
            return false;
        }

        ++_position;
        return true;
    }

    private Token Get(int relative = 0)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        return GetAbsolute(_position + relative);
    }

    private void SkipOptionalNewline()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        Match(TokenType.EndOfLine);
    }

    private Token GetAbsolute(int absolute)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        ArgumentOutOfRangeException.ThrowIfNegative(absolute);
        if (absolute < _length) return tokens[absolute];

        try
        {
            var prev = GetAbsolute(absolute - 1);
            return new Token(
                new Cursor(
                    prev.Cursor.Position,
                    prev.Cursor.Column,
                    prev.Cursor.Line,
                    prev.Cursor.Filename),
                TokenType.EndOfFile);
        }
        catch (Exception)
        {
            return new Token(
                new Cursor(0, 0, 0, ""),
                TokenType.EndOfFile);
        }
    }

    private void Trace(string message)
    {
#if PARSER_TRACE_ENABLED
        Console.WriteLine($"Trace: {message}. Get(0) = {Get()} ({Get().Cursor})"); 
#endif
    }

    private string TraceTokens()
    {
        var sb = new StringBuilder();

        var longestLineCol = tokens.Select(t => $"{t.Cursor.Line}:{t.Cursor.Column}").Max(x => x.Length) + 1;
        var selection = 10;

        foreach (var token in tokens)
        {
            var lineCol = $"{token.Cursor.Line}:{token.Cursor.Column} ";
            sb.Append(lineCol);
            if (token == Get())
            {
                sb.Append(new string('>', selection));
                sb.Append(' ');
                sb.Append(token);
                sb.Append(' ');
                sb.AppendLine(new string('<', selection));
            }
            else
            {
                sb.Append(new string(' ', selection - lineCol.Length + longestLineCol));
                sb.AppendLine(token.ToString());
            }
        }
        
        return sb.ToString();
    }
}