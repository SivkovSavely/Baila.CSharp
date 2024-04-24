using Baila.CSharp.Lexer;
using Baila.CSharp.Tests.Infrastructure;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Baila.CSharp.Tests;

public class LexerTests(ITestOutputHelper testOutputHelper) : TestsBase
{
    [Fact]
    public void SimpleProgramTest()
    {
        var lexer = new Lexer.Lexer("""
                                    var i = 5
                                    if i == 3 {
                                      print("Hello world")
                                    }
                                    """, "test.baila");

        var tokens = lexer.Tokenize();

        var i = -1;

        AssertToken(tokens, ++i, TokenType.Var);
        AssertToken(tokens, ++i, TokenType.Identifier, "i");
        AssertToken(tokens, ++i, TokenType.Eq);
        AssertToken(tokens, ++i, TokenType.NumberLiteral, "5");
        AssertToken(tokens, ++i, TokenType.EndOfLine);

        AssertToken(tokens, ++i, TokenType.If);
        AssertToken(tokens, ++i, TokenType.Identifier, "i");
        AssertToken(tokens, ++i, TokenType.EqEq);
        AssertToken(tokens, ++i, TokenType.NumberLiteral, "3");
        AssertToken(tokens, ++i, TokenType.LeftCurly);

        AssertToken(tokens, ++i, TokenType.Identifier, "print");
        AssertToken(tokens, ++i, TokenType.LeftParen);
        AssertToken(tokens, ++i, TokenType.StringLiteral, "Hello world");
        AssertToken(tokens, ++i, TokenType.RightParen);
        AssertToken(tokens, ++i, TokenType.EndOfLine);

        AssertToken(tokens, ++i, TokenType.RightCurly);
        AssertToken(tokens, ++i, TokenType.EndOfLine);

        AssertToken(tokens, ++i, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }
    
    [Fact]
    public void EndOfLineIsNotInsertedInsideParentheses()
    {
        var lexer = new Lexer.Lexer("""
                                    var i = (
                                      1,
                                      2,
                                      3
                                    )
                                    """, "test.baila");

        var tokens = lexer.Tokenize();

        var i = -1;
        AssertToken(tokens, ++i, TokenType.Var);
        AssertToken(tokens, ++i, TokenType.Identifier, "i");
        AssertToken(tokens, ++i, TokenType.Eq);
        AssertToken(tokens, ++i, TokenType.LeftParen);

        AssertToken(tokens, ++i, TokenType.NumberLiteral, "1");
        AssertToken(tokens, ++i, TokenType.Comma);

        AssertToken(tokens, ++i, TokenType.NumberLiteral, "2");
        AssertToken(tokens, ++i, TokenType.Comma);

        AssertToken(tokens, ++i, TokenType.NumberLiteral, "3");

        AssertToken(tokens, ++i, TokenType.RightParen);
        AssertToken(tokens, ++i, TokenType.EndOfLine);

        AssertToken(tokens, ++i, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }
    
    [Fact]
    public void EndOfLineIsNotInsertedInsideBrackets()
    {
        var lexer = new Lexer.Lexer("""
                                    var i = [
                                      1,
                                      2,
                                      3
                                    ]
                                    """, "test.baila");

        var tokens = lexer.Tokenize();

        var i = -1;
        AssertToken(tokens, ++i, TokenType.Var);
        AssertToken(tokens, ++i, TokenType.Identifier, "i");
        AssertToken(tokens, ++i, TokenType.Eq);
        AssertToken(tokens, ++i, TokenType.LeftBracket);

        AssertToken(tokens, ++i, TokenType.NumberLiteral, "1");
        AssertToken(tokens, ++i, TokenType.Comma);

        AssertToken(tokens, ++i, TokenType.NumberLiteral, "2");
        AssertToken(tokens, ++i, TokenType.Comma);

        AssertToken(tokens, ++i, TokenType.NumberLiteral, "3");

        AssertToken(tokens, ++i, TokenType.RightBracket);
        AssertToken(tokens, ++i, TokenType.EndOfLine);

        AssertToken(tokens, ++i, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }
    
    [Fact]
    public void EndOfLineIsInsertedInsideBrackets()
    {
        var lexer = new Lexer.Lexer("""
                                    if true {
                                      print("Hello")
                                      break
                                    }
                                    """, "test.baila");

        var tokens = lexer.Tokenize();

        var i = -1;
        AssertToken(tokens, ++i, TokenType.If);
        AssertToken(tokens, ++i, TokenType.True);
        AssertToken(tokens, ++i, TokenType.LeftCurly);

        AssertToken(tokens, ++i, TokenType.Identifier, "print");
        AssertToken(tokens, ++i, TokenType.LeftParen);
        AssertToken(tokens, ++i, TokenType.StringLiteral, "Hello");
        AssertToken(tokens, ++i, TokenType.RightParen);
        AssertToken(tokens, ++i, TokenType.EndOfLine);

        AssertToken(tokens, ++i, TokenType.Break);
        AssertToken(tokens, ++i, TokenType.EndOfLine);

        AssertToken(tokens, ++i, TokenType.RightCurly);

        AssertToken(tokens, ++i, TokenType.EndOfLine);
        AssertToken(tokens, ++i, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }
    
    [Fact]
    public void SingleStringHasNoInterpolation_LexesToSimpleToken()
    {
        var lexer = new Lexer.Lexer("""
                                    "hello world"
                                    """, "test.baila");

        var tokens = lexer.Tokenize();
        
        var i = -1;
        AssertToken(tokens, ++i, TokenType.StringLiteral, "hello world");

        AssertToken(tokens, ++i, TokenType.EndOfLine);
        AssertToken(tokens, ++i, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }
    
    [Fact]
    public void SingleStringHasSimpleNotIdentifierInterpolation_LexesToSimpleToken()
    {
        var lexer = new Lexer.Lexer("""
                                    "hello $123 world"
                                    """, "test.baila");

        var tokens = lexer.Tokenize();
        
        var i = -1;
        AssertToken(tokens, ++i, TokenType.StringLiteral, "hello $123 world");

        AssertToken(tokens, ++i, TokenType.EndOfLine);
        AssertToken(tokens, ++i, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }
    
    [Theory]
    [InlineData("i")]
    [InlineData("id")]
    [InlineData("ident")]
    public void SingleStringHasSimpleInterpolation_LexesToInterpolatedStringToken(string identifier)
    {
        var lexer = new Lexer.Lexer($"""
                                     "hello ${identifier} world"
                                     """, "test.baila");
        // compiles into concat("hello", i, " world")

        var tokens = lexer.Tokenize();
        
        var i = -1;
        AssertToken(tokens, ++i, TokenType.PrivateStringConcat);
        AssertToken(tokens, ++i, TokenType.LeftParen);
        AssertToken(tokens, ++i, TokenType.StringLiteral, "hello ");
        AssertToken(tokens, ++i, TokenType.Comma);
        AssertToken(tokens, ++i, TokenType.Identifier, identifier);
        AssertToken(tokens, ++i, TokenType.Comma);
        AssertToken(tokens, ++i, TokenType.StringLiteral, " world");
        AssertToken(tokens, ++i, TokenType.RightParen);

        AssertToken(tokens, ++i, TokenType.EndOfLine);
        AssertToken(tokens, ++i, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }

    [Fact] public void SingleStringHasComplexInterpolation_OnlyInterpolation_LexesToInterpolatedStringToken()
    {
        var lexer = new Lexer.Lexer("""
                                    "${i * 3 + 5}"
                                    """, "test.baila");
        // compiles into concat((i * 3 + 5))

        var tokens = lexer.Tokenize();
        
        var i = -1;
        AssertToken(tokens, ++i, TokenType.PrivateStringConcat);
        AssertToken(tokens, ++i, TokenType.LeftParen);
        AssertToken(tokens, ++i, TokenType.LeftParen);
        AssertToken(tokens, ++i, TokenType.Identifier, "i");
        AssertToken(tokens, ++i, TokenType.Star);
        AssertToken(tokens, ++i, TokenType.NumberLiteral, "3");
        AssertToken(tokens, ++i, TokenType.Plus);
        AssertToken(tokens, ++i, TokenType.NumberLiteral, "5");
        AssertToken(tokens, ++i, TokenType.RightParen);
        AssertToken(tokens, ++i, TokenType.RightParen);

        AssertToken(tokens, ++i, TokenType.EndOfLine);
        AssertToken(tokens, ++i, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }
    
    [Fact]
    public void SingleStringHasComplexInterpolation_HasStringAroundInterpolation_LexesToInterpolatedStringToken()
    {
        var lexer = new Lexer.Lexer("""
                                    "hello ${i * 3 + 5} world"
                                    """, "test.baila");
        // compiles into concat("hello", (i * 3 + 5), " world")

        var tokens = lexer.Tokenize();
        
        var i = -1;
        AssertToken(tokens, ++i, TokenType.PrivateStringConcat);
        AssertToken(tokens, ++i, TokenType.LeftParen);
        AssertToken(tokens, ++i, TokenType.StringLiteral, "hello ");
        AssertToken(tokens, ++i, TokenType.Comma);
        AssertToken(tokens, ++i, TokenType.LeftParen);
        AssertToken(tokens, ++i, TokenType.Identifier, "i");
        AssertToken(tokens, ++i, TokenType.Star);
        AssertToken(tokens, ++i, TokenType.NumberLiteral, "3");
        AssertToken(tokens, ++i, TokenType.Plus);
        AssertToken(tokens, ++i, TokenType.NumberLiteral, "5");
        AssertToken(tokens, ++i, TokenType.RightParen);
        AssertToken(tokens, ++i, TokenType.Comma);
        AssertToken(tokens, ++i, TokenType.StringLiteral, " world");
        AssertToken(tokens, ++i, TokenType.RightParen);

        AssertToken(tokens, ++i, TokenType.EndOfLine);
        AssertToken(tokens, ++i, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }

    private void AssertToken(List<Token> tokens, int i, TokenType type, string? value = null)
    {
        try
        {
            if (i >= tokens.Count)
            {
                Assert.Fail($"Token with type '{type.Type}' was not found in the token list.");
            }

            var token = tokens[i];
            if (type != token.Type)
            {
                Assert.Fail($"Expected token '{type.Type}', found '{token.Type.Type}'");
            }

            if (value != null && value != token.Value)
            {
                Assert.Fail($"Expected token '{token.Type.Type}' to have value '{value}', found '{token.Value}'");
            }
        }
        catch (XunitException e)
        {
            PrintTokens(tokens);
            throw;
        }
    }

    private void AssertNoMoreTokens(List<Token> tokens, int i)
    {
        try
        {
            if (i + 1 < tokens.Count)
            {
                Assert.Fail($"Expected to have no more tokens, found token {tokens[i]}");
            }
        }
        catch (XunitException e)
        {
            PrintTokens(tokens);
            throw;
        }
    }

    private void PrintTokens(List<Token> tokens)
    {
        testOutputHelper.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            testOutputHelper.WriteLine($"  {token}");
        }
    }
}