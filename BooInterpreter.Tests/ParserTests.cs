using System;
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
            var input = @"let x = 5;
                          let y = 10;
                          let foobar = 838383;";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();


            Assert.NotNull(program);
            Assert.AreEqual(3, program.Statements.Length);

            var tests = new[] {
                new { Input = "let x = 5;", ExpectedIdentifier = "x", ExpectedValue = (object)5 },
                new { Input = "let y = true;", ExpectedIdentifier = "y", ExpectedValue = (object)true },
                new { Input = "let foobar = y", ExpectedIdentifier = "foobar", ExpectedValue = (object)"y" }};

            foreach(var test in tests)
            {
                lexer = new Lexer(test.Input);
                parser = new Parser(lexer);
                program = parser.ParseProgram();
                CheckParserErrors(parser);
                Assert.AreEqual(1, program.Statements.Length);

                var statement = program.Statements.First();
                TestLetStatement(statement, test.ExpectedIdentifier);
                TestLiteralExpression(((LetStatement)statement).Value, test.ExpectedValue);
            }
        }

        private void CheckParserErrors(Parser parser)
        {
            Assert.AreEqual(0, parser.Errors.Length, $"Parser has {parser.Errors.Length} errors: {string.Join("; ", parser.Errors)}");
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
