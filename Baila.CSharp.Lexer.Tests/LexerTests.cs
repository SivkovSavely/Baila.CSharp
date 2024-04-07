using Xunit.Abstractions;
using Xunit.Sdk;

namespace Baila.CSharp.Lexer.Tests;

public class LexerTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void SimpleProgramTest()
    {
        var lexer = new Lexer("""
                              var i = 5
                              if i == 3 {
                                print("Hello world")
                              }
                              """, "test.baila");

        var tokens = lexer.Tokenize();

        var i = 0;
        
        AssertToken(tokens, i++, TokenType.Var);
        AssertToken(tokens, i++, TokenType.Identifier, "i");
        AssertToken(tokens, i++, TokenType.Eq);
        AssertToken(tokens, i++, TokenType.NumberLiteral, "5");
        AssertToken(tokens, i++, TokenType.EndOfLine);
        
        AssertToken(tokens, i++, TokenType.If);
        AssertToken(tokens, i++, TokenType.Identifier, "i");
        AssertToken(tokens, i++, TokenType.EqEq);
        AssertToken(tokens, i++, TokenType.NumberLiteral, "3");
        AssertToken(tokens, i++, TokenType.LeftCurly);

        AssertToken(tokens, i++, TokenType.Identifier, "print");
        AssertToken(tokens, i++, TokenType.LeftParen);
        AssertToken(tokens, i++, TokenType.StringLiteral, "Hello world");
        AssertToken(tokens, i++, TokenType.RightParen);
        AssertToken(tokens, i++, TokenType.EndOfLine);
        
        AssertToken(tokens, i++, TokenType.RightCurly);
        AssertToken(tokens, i++, TokenType.EndOfLine);

        AssertToken(tokens, i++, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }
    
    [Fact]
    public void EndOfLineIsNotInsertedInsideParentheses()
    {
        var lexer = new Lexer("""
                              var i = (
                                1,
                                2,
                                3
                              )
                              """, "test.baila");

        var tokens = lexer.Tokenize();

        var i = 0;
        AssertToken(tokens, i++, TokenType.Var);
        AssertToken(tokens, i++, TokenType.Identifier, "i");
        AssertToken(tokens, i++, TokenType.Eq);
        AssertToken(tokens, i++, TokenType.LeftParen);
        
        AssertToken(tokens, i++, TokenType.NumberLiteral, "1");
        AssertToken(tokens, i++, TokenType.Comma);
        
        AssertToken(tokens, i++, TokenType.NumberLiteral, "2");
        AssertToken(tokens, i++, TokenType.Comma);
        
        AssertToken(tokens, i++, TokenType.NumberLiteral, "3");
        
        AssertToken(tokens, i++, TokenType.RightParen);
        AssertToken(tokens, i++, TokenType.EndOfLine);
        
        AssertToken(tokens, i++, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }
    
    [Fact]
    public void EndOfLineIsNotInsertedInsideBrackets()
    {
        var lexer = new Lexer("""
                              var i = [
                                1,
                                2,
                                3
                              ]
                              """, "test.baila");

        var tokens = lexer.Tokenize();

        var i = 0;
        AssertToken(tokens, i++, TokenType.Var);
        AssertToken(tokens, i++, TokenType.Identifier, "i");
        AssertToken(tokens, i++, TokenType.Eq);
        AssertToken(tokens, i++, TokenType.LeftBracket);
        
        AssertToken(tokens, i++, TokenType.NumberLiteral, "1");
        AssertToken(tokens, i++, TokenType.Comma);
        
        AssertToken(tokens, i++, TokenType.NumberLiteral, "2");
        AssertToken(tokens, i++, TokenType.Comma);
        
        AssertToken(tokens, i++, TokenType.NumberLiteral, "3");
        
        AssertToken(tokens, i++, TokenType.RightBracket);
        AssertToken(tokens, i++, TokenType.EndOfLine);
        
        AssertToken(tokens, i++, TokenType.EndOfFile);
        AssertNoMoreTokens(tokens, i);
    }
    
    [Fact]
    public void EndOfLineIsInsertedInsideBrackets()
    {
        var lexer = new Lexer("""
                              if true {
                                print("Hello")
                                break
                              }
                              """, "test.baila");

        var tokens = lexer.Tokenize();

        var i = 0;
        AssertToken(tokens, i++, TokenType.If);
        AssertToken(tokens, i++, TokenType.True);
        AssertToken(tokens, i++, TokenType.LeftCurly);
        
        AssertToken(tokens, i++, TokenType.Identifier, "print");
        AssertToken(tokens, i++, TokenType.LeftParen);
        AssertToken(tokens, i++, TokenType.StringLiteral, "Hello");
        AssertToken(tokens, i++, TokenType.RightParen);
        AssertToken(tokens, i++, TokenType.EndOfLine);
        
        AssertToken(tokens, i++, TokenType.Break);
        AssertToken(tokens, i++, TokenType.EndOfLine);
        
        AssertToken(tokens, i++, TokenType.RightCurly);
        
        AssertToken(tokens, i++, TokenType.EndOfLine);
        AssertToken(tokens, i++, TokenType.EndOfFile);
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

            if (value != null)
            {
                if (value != token.Value)
                {
                    Assert.Fail($"Expected token '{token.Type.Type}' to have value '{value}', found '{token.Value}'");
                }
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
            if (i < tokens.Count)
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