using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Ast.Values;
using Baila.CSharp.Lexer;
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

        IStatement stmt;

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
            // TODO constant declaration
            throw new NotImplementedException();
        }
        else if (Match(TokenType.Function))
        {
            // TODO function declaration
            throw new NotImplementedException();
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
            // TODO stmt = expression statement
            throw new NotImplementedException();
        }
        
        // TODO require end of line or semicolon at the end of the statement
        // TODO skip end of line or semicolon token

        if (stmt == null)
        {
            throw new Exception("Stmt is null");
        }

        return stmt;
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
            stepValue = new ValueExpression(new IntValue(1));            
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
                _ => new ValueExpression(new IntValue(int.Parse(number)))
            };
        }
        // Strings
        else if (Match(TokenType.StringLiteral))
        {
            result = new ValueExpression(new StringValue(current.Value!));
        }
        // true and false
        else if (Match(TokenType.True))
        {
            result = new ValueExpression(new BooleanValue(true));
        }
        else if (Match(TokenType.False))
        {
            result = new ValueExpression(new BooleanValue(false));
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