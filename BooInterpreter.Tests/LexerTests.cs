﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BooInterpreter
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void Lexer_NextToken()
        {
            var input = @"let five = 5;
                          let ten = 10;
                          
                          let add = fn(x, y) {
                              x + y;
                          };
                          
                          let result = add(five, ten);
                          !-/*5;
                          5 < 10 > 5;
                          
                          if (5 < 19) {
                              return true;
                          } else {
                              return false;
                          }
                          
                          10 == 10;
                          10 != 9;
                          
                          ""foobar""
                          ""foo bar""

                          [1, 2];

                          { ""foo"": ""bar"" }
                          "; 
        
            var tests = new(TokenType expectedType, string expectedLiteral)[] { 
                (expectedType: TokenType.LET, expectedLiteral: "let"),
                (expectedType: TokenType.IDENT, expectedLiteral: "five"),
                (expectedType: TokenType.ASSIGN, expectedLiteral: "="),
                (expectedType: TokenType.INT, expectedLiteral: "5"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),

                (expectedType: TokenType.LET, expectedLiteral: "let"),
                (expectedType: TokenType.IDENT, expectedLiteral: "ten"),
                (expectedType: TokenType.ASSIGN, expectedLiteral: "="),
                (expectedType: TokenType.INT, expectedLiteral: "10"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),

                (expectedType: TokenType.LET, expectedLiteral: "let"),
                (expectedType: TokenType.IDENT, expectedLiteral: "add"),
                (expectedType: TokenType.ASSIGN, expectedLiteral: "="),
                (expectedType: TokenType.FUNCTION, expectedLiteral: "fn"),
                (expectedType: TokenType.LPAREN, expectedLiteral: "("),
                (expectedType: TokenType.IDENT, expectedLiteral: "x"),
                (expectedType: TokenType.COMMA, expectedLiteral: ","),
                (expectedType: TokenType.IDENT, expectedLiteral: "y"),
                (expectedType: TokenType.RPAREN, expectedLiteral: ")"),
                (expectedType: TokenType.LBRACE, expectedLiteral: "{"),
                (expectedType: TokenType.IDENT, expectedLiteral: "x"),
                (expectedType: TokenType.PLUS, expectedLiteral: "+"),
                (expectedType: TokenType.IDENT, expectedLiteral: "y"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),
                (expectedType: TokenType.RBRACE, expectedLiteral: "}"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),

                (expectedType: TokenType.LET, expectedLiteral: "let"),
                (expectedType: TokenType.IDENT, expectedLiteral: "result"),
                (expectedType: TokenType.ASSIGN, expectedLiteral: "="),
                (expectedType: TokenType.IDENT, expectedLiteral: "add"),
                (expectedType: TokenType.LPAREN, expectedLiteral: "("),
                (expectedType: TokenType.IDENT, expectedLiteral: "five"),
                (expectedType: TokenType.COMMA, expectedLiteral: ","),
                (expectedType: TokenType.IDENT, expectedLiteral: "ten"),
                (expectedType: TokenType.RPAREN, expectedLiteral: ")"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),

                (expectedType: TokenType.BANG, expectedLiteral: "!"),
                (expectedType: TokenType.MINUS, expectedLiteral: "-"),
                (expectedType: TokenType.SLASH, expectedLiteral: "/"),
                (expectedType: TokenType.ASTERISK, expectedLiteral: "*"),
                (expectedType: TokenType.INT, expectedLiteral: "5"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),

                (expectedType: TokenType.INT, expectedLiteral: "5"),
                (expectedType: TokenType.LT, expectedLiteral: "<"),
                (expectedType: TokenType.INT, expectedLiteral: "10"),
                (expectedType: TokenType.GT, expectedLiteral: ">"),
                (expectedType: TokenType.INT, expectedLiteral: "5"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),

                (expectedType: TokenType.IF, expectedLiteral: "if"),
                (expectedType: TokenType.LPAREN, expectedLiteral: "("),
                (expectedType: TokenType.INT, expectedLiteral: "5"),
                (expectedType: TokenType.LT, expectedLiteral: "<"),
                (expectedType: TokenType.INT, expectedLiteral: "19"),
                (expectedType: TokenType.RPAREN, expectedLiteral: ")"),
                (expectedType: TokenType.LBRACE, expectedLiteral: "{"),
                (expectedType: TokenType.RETURN, expectedLiteral: "return"),
                (expectedType: TokenType.TRUE, expectedLiteral: "true"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),
                (expectedType: TokenType.RBRACE, expectedLiteral: "}"),
                (expectedType: TokenType.ELSE, expectedLiteral: "else"),
                (expectedType: TokenType.LBRACE, expectedLiteral: "{"),
                (expectedType: TokenType.RETURN, expectedLiteral: "return"),
                (expectedType: TokenType.FALSE, expectedLiteral: "false"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),
                (expectedType: TokenType.RBRACE, expectedLiteral: "}"),

                (expectedType: TokenType.INT, expectedLiteral: "10"),
                (expectedType: TokenType.EQ, expectedLiteral: "=="),
                (expectedType: TokenType.INT, expectedLiteral: "10"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),

                (expectedType: TokenType.INT, expectedLiteral: "10"),
                (expectedType: TokenType.NOT_EQ, expectedLiteral: "!="),
                (expectedType: TokenType.INT, expectedLiteral: "9"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),
                
                (expectedType: TokenType.STRING, expectedLiteral: "foobar"),
                (expectedType: TokenType.STRING, expectedLiteral: "foo bar"),

                (expectedType: TokenType.LBRACKET, expectedLiteral: "["),
                (expectedType: TokenType.INT, expectedLiteral: "1"),
                (expectedType: TokenType.COMMA, expectedLiteral: ","),
                (expectedType: TokenType.INT, expectedLiteral: "2"),
                (expectedType: TokenType.RBRACKET, expectedLiteral: "]"),
                (expectedType: TokenType.SEMICOLON, expectedLiteral: ";"),

                (expectedType: TokenType.LBRACE, expectedLiteral: "{"),
                (expectedType: TokenType.STRING, expectedLiteral: "foo"),
                (expectedType: TokenType.COLON, expectedLiteral: ":"),
                (expectedType: TokenType.STRING, expectedLiteral: "bar"),
                (expectedType: TokenType.RBRACE, expectedLiteral: "}"),
                
                (expectedType: TokenType.EOF, expectedLiteral: ""),
            };

            var lexer = new Lexer(input);

            foreach(var test in tests)
            {
                var token = lexer.NextToken();
                Assert.AreEqual(test.expectedType, token.Type);
                Assert.AreEqual(test.expectedLiteral, token.Literal);
            }            
        }
    }
}
