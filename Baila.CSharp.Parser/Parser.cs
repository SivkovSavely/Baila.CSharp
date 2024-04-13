﻿using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Lexer;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Parser;

public class Parser(List<Token> tokens)
{
    private int _position;
    private readonly int _length = tokens.Count;

    public IStatement BuildAst()
    {
        var result = new BlockStatement();

        while (!Match(TokenType.EndOfFile))
        {
            result.AddStatement(Statement());
        }

        return result;
    }

    private IStatement Statement()
    {
        if (Match(TokenType.EndOfLine) || Match(TokenType.Semicolon))
        {
            return new NoOpStatement();
        }

        IStatement stmt = null!;

        if (Match(TokenType.If))
        {
            stmt = IfElseStatement();
        }
        else if (Match(TokenType.For))
        {
            stmt = ForStatement();
        }
        else if (Match(TokenType.While))
        {
            stmt = WhileStatement();
        }
        else if (Match(TokenType.Do))
        {
            stmt = DoWhileStatement();
        }
        else if (Match(TokenType.Var))
        {
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
            var name = Consume(TokenType.Identifier).Value!;
            Consume(TokenType.Eq);
            var value = Expression();

            stmt = new ConstantDefineStatement(name, value);
        }
        else if (Match(TokenType.Function))
        {
            stmt = FunctionDefinition();
        }
        else if (Match(TokenType.Return))
        {
            // TODO return statement
            throw new NotImplementedException();
        }
        else if (Match(TokenType.Break))
        {
            // TODO break statement
            throw new NotImplementedException();
        }
        else if (Match(TokenType.Continue))
        {
            // TODO continue statement
            throw new NotImplementedException();
        }
        else if (Match(TokenType.Class))
        {
            // TODO class declaration
            throw new NotImplementedException();
        }
        else
        {
            stmt = new ExpressionStatement(Expression());
        }

        if (!LookMatch(0, TokenType.EndOfLine) &&
            !LookMatch(0, TokenType.Semicolon) &&
            !LookMatch(0, TokenType.EndOfFile))
        {
            throw new Exception($"Syntax error: unexpected token {Get()}, expected a new line or a semicolon");
        }

        if (LookMatch(0, TokenType.EndOfLine)) Consume(TokenType.EndOfLine);
        else if (LookMatch(0, TokenType.Semicolon)) Consume(TokenType.Semicolon);
        else if (LookMatch(0, TokenType.EndOfFile)) Consume(TokenType.EndOfFile);

        if (stmt == null)
        {
            throw new Exception("Stmt is null");
        }

        return stmt;
    }

    private FunctionDefineStatement FunctionDefinition()
    {
        var name = Consume(TokenType.Identifier).Value!;
        var parameters = new List<FunctionDefineStatement.FunctionParameter>();
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
                    new FunctionDefineStatement.FunctionParameter(
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

        if (returnType == null)
        {
            throw new NotImplementedException("Inferring the return type is not supported yet");
        }

        return new FunctionDefineStatement(
            name, parameters, body, returnType);
    }

    private BailaType Type()
    {
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
        var condition = Expression();
        var trueStmt = StatementOrBlock();
        IStatement? falseStmt = null;
        if (Match(TokenType.Else))
        {
            falseStmt = StatementOrBlock();
        }

        return new IfElseStatement(condition, trueStmt, falseStmt);
    }

    private IStatement ForStatement()
    {
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
        var condition = Expression();
        var body = StatementOrBlock();

        return new WhileStatement(condition, body);
    }

    private IStatement DoWhileStatement()
    {
        var body = StatementOrBlock();
        Consume(TokenType.While);
        var condition = Expression();

        return new DoWhileStatement(condition, body);
    }

    private IStatement StatementOrBlock()
    {
        if (LookMatch(0, TokenType.LeftCurly))
        {
            return StatementBlock();
        }

        return Statement();
    }

    private IStatement StatementBlock()
    {
        var block = new BlockStatement();
        Consume(TokenType.LeftCurly);
        while (!Match(TokenType.RightCurly))
        {
            block.AddStatement(Statement());
        }

        return block;
    }

    private Token Consume(TokenType tokenType)
    {
        var current = Get();
        if (current.Type != tokenType)
        {
            throw new Exception($"Syntax error: unexpected token {current.Type}, expected {tokenType}");
        }

        _position++;
        return current;
    }

    private bool LookMatch(int relative, TokenType tokenType)
    {
        return Get(relative).Type == tokenType;
    }

    private IExpression Expression()
    {
        return Assignment();
    }

    private IExpression Assignment()
    {
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

    // TODO binaryOr
    // TODO binaryAnd

    private IExpression BitwiseOr()
    {
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
        var result = Equality();

        while (true)
        {
            if (Match(TokenType.Amp))
            {
                result = new BinaryExpression(BinaryExpression.Operation.BitwiseAnd, result, Equality());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression Equality()
    {
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

        return Primary();
    }

    private IExpression Primary()
    {
        var current = Get();
        IExpression? result = null;

        // (expr)
        if (Match(TokenType.LeftParen))
        {
            var expression = Expression();
            Consume(TokenType.RightParen);
            result = expression;
        }
        // [ listElem1, listElem2, ..., listElemN ]
        else if (Match(TokenType.LeftBracket))
        {
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
            var currentNum = current.Value!;
            var suffix = currentNum.Last();
            var number = suffix is 'c' ? currentNum[..^1] : currentNum;

            result = suffix switch
            {
                'c' => throw new NotImplementedException("Char number literals are not implemented yet"),
                _ => new IntValueExpression(int.Parse(number))
            };
        }
        // Strings
        else if (Match(TokenType.StringLiteral))
        {
            result = new StringValueExpression(current.Value!);
        }
        // true and false
        else if (Match(TokenType.True))
        {
            result = new BoolValueExpression(true);
        }
        else if (Match(TokenType.False))
        {
            result = new BoolValueExpression(false);
        }
        // Variables and constants
        else if (Match(TokenType.Identifier))
        {
            result = new VariableExpression(current.Value!);
        }

        if (result == null)
        {
            throw new Exception($"Syntax error: unexpected {current}");
        }

        if (LookMatch(0, TokenType.LeftParen)) // TODO do this in the infinite loop, alongside LeftBracket and Dot
        {
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
        if (Get().Type != tokenType)
        {
            return false;
        }

        ++_position;
        return true;
    }

    private Token Get(int relative = 0)
    {
        return GetAbsolute(_position + relative);
    }

    private void SkipOptionalNewline()
    {
        Match(TokenType.EndOfLine);
    }

    private Token GetAbsolute(int absolute)
    {
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
}