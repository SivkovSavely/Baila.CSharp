// #define PARSER_TRACE_ENABLED

using System.Globalization;
using System.Text;
using Baila.CSharp.Ast.Diagnostics;
using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Syntax;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Ast.Syntax.Statements;
using Baila.CSharp.Lexer;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Parser;

public class Parser(string filename, string source, List<Token> tokens, CancellationToken? cancellationToken = null)
{
    private int _position;
    private readonly int _length = tokens.Count;

    private readonly Stack<int> _rollbackPositions = new();
    private readonly string _filename = filename;
    private readonly List<ParserDiagnostic> _diagnostics = [];

    public IEnumerable<ParserDiagnostic> Diagnostics => _diagnostics.AsReadOnly();

    public Statements BuildAst()
    {
        var result = new Statements();

        while (!Match(TokenType.EndOfFile))
        {
            var stmt = Statement();
            result.AddStatement(stmt);
        }

        return result;
    }

    private IStatement Statement(bool requireEndOfStatement = true)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        IStatement stmt = null!;

        if (Match(TokenType.If, out var ifKeyword))
        {
            Trace("IfElseStatement");
            stmt = IfElseStatement(ifKeyword);
        }
        else if (Match(TokenType.For, out var forKeyword))
        {
            Trace("ForStatement");
            stmt = ForStatement(forKeyword);
        }
        else if (Match(TokenType.While, out var whileKeyword))
        {
            Trace("WhileStatement");
            stmt = WhileStatement(whileKeyword);
        }
        else if (Match(TokenType.Do, out var doKeyword))
        {
            Trace("DoWhileStatement");
            stmt = DoWhileStatement(doKeyword);
        }
        else if (Match(TokenType.Var, out var varKeyword))
        {
            Trace("VariableDefineStatement");
            var nameIdentifier = Consume(TokenType.Identifier);
            BailaType? type = null;
            IExpression? value = null;

            SyntaxNodeSpan? typeSpan = null;
            if (Match(TokenType.Colon))
            {
                type = Type(out var typeSpan2);
                typeSpan = typeSpan2;
            }

            if (Match(TokenType.Eq))
            {
                value = Expression();
            }

            stmt = new VariableDefineStatement(varKeyword, nameIdentifier, type, typeSpan, value);
        }
        else if (Match(TokenType.Const))
        {
            Trace("ConstantDefineStatement");
            var constKeyword = Get(-1);
            var nameIdentifier = Consume(TokenType.Identifier);
            var equalsToken = Consume(TokenType.Eq);
            var value = Expression();

            stmt = new ConstantDefineStatement(constKeyword, nameIdentifier, equalsToken, value);
        }
        else if (Match(TokenType.Function, out var functionKeyword))
        {
            Trace("FunctionDefinition");
            stmt = FunctionDefinition(functionKeyword);
        }
        else if (Match(TokenType.Return, out var returnToken))
        {
            Trace("ReturnStatement");
            if (LookMatch(0, TokenType.EndOfFile) || LookMatch(0, TokenType.Semicolon) || LookMatch(0, TokenType.EndOfLine) || LookMatch(0, TokenType.RightCurly))
            {
                stmt = new ReturnStatement(returnToken, null);
            }
            else
            {
                stmt = new ReturnStatement(returnToken, Expression());
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
            return new NoOpStatement(Get(-1).Span);
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
                AddDiagnostic<Token, TokenType[]>(
                    ParserDiagnostics.BP0001_UnexpectedToken,
                    Get(),
                    [TokenType.EndOfLine, TokenType.Semicolon],
                    underlinedNode: Get());
                throw new ParseException(_diagnostics);
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

    private FunctionDefineStatement FunctionDefinition(Token functionKeyword)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var functionNameIdentifier = Consume(TokenType.Identifier);
        var parameters = new List<FunctionParameter>();
        BailaType? returnType = null;

        Token? leftParen = null;
        Token? rightParen = null;
        if (Match(TokenType.LeftParen, out leftParen))
        {
            // TODO varargs
            while (!Match(TokenType.RightParen))
            {
                var paramName = Consume(TokenType.Identifier).Value!;
                Consume(TokenType.Colon);
                var paramType = Type(out _);
                IExpression? defaultValue = null;
                if (Match(TokenType.Eq))
                {
                    defaultValue = Expression();
                }

                parameters.Add(
                    new FunctionParameter(
                        paramName, paramType, defaultValue, false));

                if (Match(TokenType.RightParen, out rightParen)) break;

                Consume(TokenType.Comma);
            }
        }

        var shouldTraverseBody = false;
        SkipOptionalNewline();
        if (Match(TokenType.Colon)) {
            // TODO redo to catch fully optional expression
            SkipOptionalNewline();
            returnType = Type(out _);
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
            functionKeyword, functionNameIdentifier, leftParen, parameters, rightParen, returnType, body);
    }

    private BailaType Type(out SyntaxNodeSpan typeSpan)
    {
        cancellationToken?.ThrowIfCancellationRequested();
        SyntaxNodeSpan? span = null;

        // nullable check
        var isNullable = Match(TokenType.Question, out var questionToken);
        if (isNullable)
        {
            span = questionToken.Span;
        }
        
        // generics
        List<BailaType>? genericsList = null;
        if (Match(TokenType.Lt, out var genericLtToken)) // TODO support LtLt e.g. <<Int>List>List
        {
            span = span.HasValue ? SyntaxNodeSpan.Merge(span.Value, genericLtToken.Span) : genericLtToken.Span;
            genericsList = new();

            Token genericGtToken;
            while (!Match(TokenType.Gt, out genericGtToken))
            {
                var genericType = Type(out var genericTypeSpan);
                span = SyntaxNodeSpan.Merge(span.Value, genericTypeSpan);

                genericsList.Add(genericType);

                if (Match(TokenType.Gt)) break;

                Consume(TokenType.Comma);
            }
            
            span = SyntaxNodeSpan.Merge(span.Value, genericGtToken.Span);
        }
        
        // type name
        var typeNameIdentifier = Consume(TokenType.Identifier);
        span = span.HasValue ? SyntaxNodeSpan.Merge(span.Value, typeNameIdentifier.Span) : typeNameIdentifier.Span;

        var result = new BailaType(typeNameIdentifier.Value!, isNullable, genericsList);
        typeSpan = span.Value;
        return result;
    }

    private IStatement IfElseStatement(Token ifKeyword)
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

        return new IfElseStatement(ifKeyword, condition, trueStmt, falseStmt);
    }

    private IStatement ForStatement(Token forKeyword)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var optionalLeftParen = Match(TokenType.LeftParen);

        var counterVariableIdentifier = Consume(TokenType.Identifier);
        var equalsToken = Consume(TokenType.Eq);
        var initialValue = Expression();

        if (!(Get().Type == TokenType.Identifier && Get().Value == "to"))
        {
            AddDiagnostic(
                ParserDiagnostics.BP0002_ExpectedToInForLoop,
                Get(),
                underlinedNode: Get());
            throw new ParseException(_diagnostics);
        }

        Match(TokenType.Identifier, out var toSoftKeyword);

        var finalValue = Expression();

        IExpression stepValue;
        if (Get().Type == TokenType.Identifier && Get().Value == "step")
        {
            Match(TokenType.Identifier);
            stepValue = Expression();
        }
        else
        {
            stepValue = new IntValueExpression(1, forKeyword);
        }

        if (optionalLeftParen)
        {
            Match(TokenType.RightParen);
        }

        var body = StatementOrBlock();

        return new ForStatement(
            forKeyword, counterVariableIdentifier, equalsToken, initialValue, toSoftKeyword, finalValue, stepValue, body);
    }

    private IStatement WhileStatement(Token whileKeyword)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var condition = Expression();
        var body = StatementOrBlock();

        return new WhileStatement(whileKeyword, condition, body);
    }

    private IStatement DoWhileStatement(Token doKeyword)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var body = StatementOrBlock();
        var whileKeyword = Consume(TokenType.While);
        var condition = Expression();

        return new DoWhileStatement(doKeyword, body, whileKeyword, condition);
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

        var leftCurlyToken = Consume(TokenType.LeftCurly);
        var statements = new List<IStatement>();
        Token rightCurlyToken;

        while (!Match(TokenType.RightCurly, out rightCurlyToken))
        {
            statements.Add(Statement(requireEndOfStatement));
        }

        var block = new BlockStatement(leftCurlyToken, statements, rightCurlyToken);
        return block;
    }

    private Token Consume(TokenType tokenType)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var current = Get();
        if (current.Type != tokenType)
        {
            AddDiagnostic<Token, TokenType[]>(
                ParserDiagnostics.BP0001_UnexpectedToken,
                current, [tokenType],
                underlinedNode: current);
            throw new ParseException(_diagnostics);
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
        var expr = Assignment();
        
        return expr;
    }

    private IExpression Assignment()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        if (LookMatch(0, TokenType.Identifier) && LookMatch(1, TokenType.Eq))
        {
            var nameIdentifierToken = Consume(TokenType.Identifier);
            var name = nameIdentifierToken.Value!;
            var equalsSign = Consume(TokenType.Eq);
            var expr = Assignment();

            var target = new VariableExpression(name, nameIdentifierToken);
            return new AssignmentExpression(target, equalsSign, expr);
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
                result = new BinaryExpression(
                    BinaryExpression.Operation.BitwiseOr,
                    result, 
                    BitwiseXor());
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
                result = new BinaryExpression(
                    BinaryExpression.Operation.BitwiseXor,
                    result, 
                    BitwiseAnd());
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
                result = new BinaryExpression(BinaryExpression.Operation.BitwiseAnd,
                    result,
                    LogicalOr());
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
                result = new BinaryExpression(
                    BinaryExpression.Operation.LogicalOr,
                    result,
                    LogicalAnd());
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
                result = new BinaryExpression(
                    BinaryExpression.Operation.LogicalAnd,
                    result,
                    Equality());
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
                result = new BinaryExpression(
                    BinaryExpression.Operation.Equality,
                    result,
                    NumberRelation());
                continue;
            }

            if (Match(TokenType.ExclEq))
            {
                result = new BinaryExpression(
                    BinaryExpression.Operation.Inequality,
                    result,
                    NumberRelation());
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
                result = new BinaryExpression(
                    BinaryExpression.Operation.LessThan,
                    result,
                    Addition());
                continue;
            }

            if (Match(TokenType.LtEq))
            {
                result = new BinaryExpression(
                    BinaryExpression.Operation.LessThanOrEqual,
                    result,
                    Addition());
                continue;
            }

            if (Match(TokenType.Gt))
            {
                result = new BinaryExpression(
                    BinaryExpression.Operation.GreaterThan,
                    result,
                    Addition());
                continue;
            }

            if (Match(TokenType.GtEq))
            {
                result = new BinaryExpression(
                    BinaryExpression.Operation.GreaterThanOrEqual,
                    result,
                    Addition());
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
                result = new BinaryExpression(
                    BinaryExpression.Operation.Addition,
                    result,
                    Multiplication());
                continue;
            }

            if (Match(TokenType.Minus))
            {
                result = new BinaryExpression(
                    BinaryExpression.Operation.Subtraction,
                    result,
                    Multiplication());
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
                result = new BinaryExpression(
                    BinaryExpression.Operation.FloatDivision,
                    result,
                    Power());
                continue;
            }

            if (Match(TokenType.SlashSlash))
            {
                result = new BinaryExpression(
                    BinaryExpression.Operation.IntegerDivision,
                    result,
                    Power());
                continue;
            }

            if (Match(TokenType.Star))
            {
                result = new BinaryExpression(
                    BinaryExpression.Operation.Multiplication,
                    result,
                    Power());
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
                result = new BinaryExpression(
                    BinaryExpression.Operation.Power,
                    result,
                    Unary());
                continue;
            }

            break;
        }

        return result;
    }

    private IExpression Unary()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        Token operatorToken;

        if (Match(TokenType.Tilde, out operatorToken))
        {
            return new PrefixUnaryExpression(
                PrefixUnaryExpression.Operation.BitwiseNegation,
                operatorToken,
                Unary());
        }

        if (Match(TokenType.Excl, out operatorToken))
        {
            return new PrefixUnaryExpression(
                PrefixUnaryExpression.Operation.LogicalNegation,
                operatorToken,
                Unary());
        }

        if (Match(TokenType.Plus, out operatorToken))
        {
            return new PrefixUnaryExpression(
                PrefixUnaryExpression.Operation.Plus,
                operatorToken,
                Unary());
        }

        if (Match(TokenType.Minus, out operatorToken))
        {
            return new PrefixUnaryExpression(
                PrefixUnaryExpression.Operation.Minus,
                operatorToken,
                Unary());
        }

        if (Match(TokenType.Typeof, out var typeofKeyword))
        {
            return new TypeOfExpression(typeofKeyword, Unary());
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
        if (Match(TokenType.LeftParen, out var leftParenToken))
        {
            Trace("LeftParen");
            var expression = Expression();
            var rightParenToken = Consume(TokenType.RightParen);
            result = new ParenthesizedExpression(leftParenToken, expression, rightParenToken);
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
        else if (Match(TokenType.NumberLiteral, out var numberLiteralToken))
        {
            Trace("NumberLiteral");
            var currentNum = numberLiteralToken.Value!;
            var suffix = currentNum.Last();
            var number = suffix is 'c' ? currentNum[..^1] : currentNum;

            result = suffix switch
            {
                'c' => throw new NotImplementedException("Char number literals are not implemented yet"),
                _ when long.TryParse(number, CultureInfo.InvariantCulture, out var intNumber) =>
                    new IntValueExpression(intNumber, numberLiteralToken),
                _ when double.TryParse(number, CultureInfo.InvariantCulture, out var floatNumber) =>
                    new FloatValueExpression(floatNumber, numberLiteralToken),
                _ => null
            };

            if (result == null)
            {
                AddDiagnostic(
                    ParserDiagnostics.BP0003_CountNotInferTypeOfNumber,
                    Get(-1),
                    underlinedNode: Get(-1));
                throw new ParseException(_diagnostics);
            }
        }
        // Strings
        else if (Match(TokenType.StringLiteral, out var stringLiteralToken))
        {
            Trace("StringLiteral");
            result = new StringValueExpression(stringLiteralToken.Value!, stringLiteralToken);
        }
        // Interpolated Strings
        else if (Match(TokenType.PrivateStringConcat))
        {
            Trace("PrivateStringConcat");
            var fixedStrings = new List<string>();
            var expressions = new List<IExpression>();
            bool? expressionWasLast = null;

            var startToken = Consume(TokenType.PrivateStringConcatStart);
            Token endToken;

            while (true)
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

                if (Match(TokenType.PrivateStringConcatEnd, out endToken)) break;

                Consume(TokenType.Comma);
            }
            
            result = new StringConcatExpression(startToken, fixedStrings, expressions, endToken);
        }
        // true and false
        else if (Match(TokenType.True, out var trueLiteralToken))
        {
            Trace("True");
            result = new BoolValueExpression(true, trueLiteralToken);
        }
        else if (Match(TokenType.False, out var falseLiteralToken))
        {
            Trace("False");
            result = new BoolValueExpression(false, falseLiteralToken);
        }
        // Variables and constants
        else if (Match(TokenType.Identifier, out var identifierToken))
        {
            Trace("Identifier");
            result = new VariableExpression(identifierToken.Value!, identifierToken);
        }

        if (result == null)
        {
            AddDiagnostic(ParserDiagnostics.BP0001_UnexpectedToken,
                current,
                "end of statement, call, array access or dot access",
                underlinedNode: current);
            throw new ParseException(_diagnostics);
        }

        cancellationToken?.ThrowIfCancellationRequested();

        if (LookMatch(0, TokenType.LeftParen)) // TODO do this in the infinite loop, alongside LeftBracket and Dot
        {
            Trace("LeftParen after");
            var args = new List<IExpression>();
            var leftParen = Consume(TokenType.LeftParen);
            Token? rightParen = null;

            while (!Match(TokenType.EndOfFile))
            {
                args.Add(Expression());
                if (Match(TokenType.RightParen, out rightParen))
                {
                    break;
                }

                Consume(TokenType.Comma);
            }

            result = new FunctionCallExpression(result, leftParen, args, rightParen);
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

    private bool Match(TokenType tokenType, out Token outToken)
    {
        cancellationToken?.ThrowIfCancellationRequested();

        var token = Get();
        if (token.Type != tokenType)
        {
            outToken = null!;
            return false;
        }

        outToken = token;
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
                filename,
                prev.Span,
                TokenType.EndOfFile);
        }
        catch (Exception)
        {
            return new Token(
                _filename,
                SyntaxNodeSpan.ThrowEmpty,
                TokenType.EndOfFile);
        }
    }

    private void AddDiagnostic<TParam>(
        Func<TParam, string[], ParserDiagnostic> diagnosticCreator,
        TParam param,
        ISyntaxNode underlinedNode)
    {
        var diagnostic = diagnosticCreator(param, GetRelevantLines(underlinedNode));
        _diagnostics.Add(diagnostic);
    }

    private void AddDiagnostic<TParam1, TParam2>(
        Func<TParam1, TParam2, string[], ParserDiagnostic> diagnosticCreator,
        TParam1 param1, TParam2 param2,
        ISyntaxNode underlinedNode)
    {
        var diagnostic = diagnosticCreator(param1, param2, GetRelevantLines(underlinedNode));
        _diagnostics.Add(diagnostic);
    }

    private void AddDiagnostic<TParam1, TParam2, TParam3>(
        Func<TParam1, TParam2, TParam3, string[], ParserDiagnostic> diagnosticCreator,
        TParam1 param1, TParam2 param2, TParam3 param3,
        ISyntaxNode underlinedNode)
    {
        var diagnostic = diagnosticCreator(param1, param2, param3, GetRelevantLines(underlinedNode));
        _diagnostics.Add(diagnostic);
    }

    private string[] GetRelevantLines(ISyntaxNode node)
    {
        var startLine = node.Span.StartLine;
        var endLineInclusive = node.Span.EndLine;
        var lines = source.Split("\n")[(startLine - 1)..endLineInclusive]; // TODO
        return lines;
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

        var longestLineCol = tokens.Select(t => $"{t.Span.StartLine}:{t.Span.StartColumn}").Max(x => x.Length) + 1;
        var selection = 10;

        foreach (var token in tokens)
        {
            var lineCol = $"{token.Span.StartLine}:{token.Span.StartColumn} ";
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