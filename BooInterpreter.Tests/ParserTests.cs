﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BooInterpreter
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Parser_LetStatements()
        {
            var tests = new[] {
                new { Input = "let x = 5;", ExpectedIdentifier = "x", ExpectedValue = (object)5 },
                new { Input = "let y = true;", ExpectedIdentifier = "y", ExpectedValue = (object)true },
                new { Input = "let foobar = y", ExpectedIdentifier = "foobar", ExpectedValue = (object)"y" },
            };

            foreach(var test in tests)
            {
                var lexer = new Lexer(test.Input);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                CheckParserErrors(parser);
                Assert.AreEqual(1, program.Statements.Length);

                var statement = program.Statements.First() as LetStatement;
                Assert.NotNull(statement);
                TestLetStatement(statement, test.ExpectedIdentifier);
                TestLiteralExpression(((LetStatement)statement).Value, test.ExpectedValue);
            }
        }

        [Test]
        public void Parser_ReturnStatements()
        {
            var tests = new[] {
                new { Input = "return 5;", ExpectedReturnValue = (object)5},
                new { Input = "return 5;", ExpectedReturnValue = (object)10},
                new { Input = "return 5;", ExpectedReturnValue = (object)993322},
            };

            foreach (var test in tests)
            {
                var lexer = new Lexer(test.Input);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                CheckParserErrors(parser);
                Assert.AreEqual(1, program.Statements.Length);

                var statement = program.Statements.First() as ReturnStatement;
                Assert.NotNull(statement);
                Assert.AreEqual("return", statement.TokenLiteral);
                TestLiteralExpression(statement.ReturnValue, test.ExpectedReturnValue);
            }
        }

        private void CheckParserErrors(Parser parser)
        {
            Assert.AreEqual(0, parser.Errors.Length, $"Parser has {parser.Errors.Length} errors: {string.Join("\r\n", parser.Errors)}");
        }

        private void TestLetStatement(Statement statement, string expectedName)
        {
            Assert.IsInstanceOf<LetStatement>(statement);
            Assert.AreEqual("let", statement.TokenLiteral);
            Assert.AreEqual(expectedName, ((LetStatement)statement).Name.TokenLiteral);
        }

        private void TestLiteralExpression(Expression expression, object value)
        {
            
        }


    }
}
