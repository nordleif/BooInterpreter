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
        public void Parser_ToString()
        {
            var program = new Program {
                Statements = new Statement[] {
                    new LetStatement {
                        Token = new Token(TokenType.LET, "let"),
                        Name = new Identifier { Token = new Token(TokenType.IDENT, "myVar"), Value = "myVar" },
                        Value = new Identifier { Token = new Token(TokenType.IDENT, "anotherVar"), Value = "anotherVar" }
                    }
                }
            };

            Assert.AreEqual("let myVar = anotherVar;", program.ToString());
        }

        [Test]
        public void Parser_LetStatements()
        {
            var tests = new[] {
                new { Input = "let x = 5;", ExpectedIdentifier = "x", ExpectedValue = (object)5L },
                //new { Input = "let y = true;", ExpectedIdentifier = "y", ExpectedValue = (object)true },
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
                new { Input = "return 5;", ExpectedReturnValue = (object)5L},
                new { Input = "return 10;", ExpectedReturnValue = (object)10L},
                new { Input = "return 993322;", ExpectedReturnValue = (object)993322L},
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

        [Test]
        public void Parser_IdentifierExpression()
        {
            var input = "foobar;";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);
            Assert.AreEqual(1, program.Statements.Length);

            var statement = program.Statements.First() as ExpressionStatement;
            Assert.IsNotNull(statement);
            Assert.IsNotNull(statement.Expression);

            var identifier = statement.Expression as Identifier;
            Assert.IsNotNull(identifier);
            Assert.AreEqual("foobar", identifier.Value);
            Assert.AreEqual("foobar", identifier.TokenLiteral);
        }

        [Test]
        public void Parser_IntegerLiteralExpression()
        {
            var input = "5;";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);
            Assert.AreEqual(1, program.Statements.Length);

            var statement = program.Statements.First() as ExpressionStatement;
            Assert.IsNotNull(statement);
            Assert.IsNotNull(statement.Expression);

            var expression = statement.Expression as IntegerLiteral;
            Assert.IsNotNull(expression);
            Assert.AreEqual(5, expression.Value);
            Assert.AreEqual("5", expression.TokenLiteral);
        }

        [Test]
        public void Parser_PrefixExpressions()
        {
            var tests = new[] {
                new { Input = "!5;", Operator = "!", Value = (object)5L },
                new { Input = "-15;", Operator = "-", Value = (object)15L },
                new { Input = "!true;", Operator = "!", Value = (object)true },
                new { Input = "!false;", Operator = "!",  Value = (object)false }
            };

            foreach (var test in tests)
            {
                var lexer = new Lexer(test.Input);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                CheckParserErrors(parser);
                Assert.AreEqual(1, program.Statements.Length);

                var statement = program.Statements[0] as ExpressionStatement;
                Assert.IsNotNull(statement);

                var expression = statement.Expression as PrefixExpression;
                Assert.IsNotNull(expression);
                Assert.AreEqual(test.Operator, expression.Operator);

                TestLiteralExpression(expression.Right, test.Value);
            }
        }

        [Test]
        public void Parser_InfixExpressions()
        {
            var tests = new[] {
                new { Input = "5 + 5;", LeftValue = (object)5L, Operator = "+", RightValue = (object)5L },
                new { Input = "5 - 5;", LeftValue = (object)5L, Operator = "-", RightValue = (object)5L },
                new { Input = "5 * 5;", LeftValue = (object)5L, Operator = "*", RightValue = (object)5L },
                new { Input = "5 / 5;", LeftValue = (object)5L, Operator = "/", RightValue = (object)5L },
                new { Input = "5 > 5;", LeftValue = (object)5L, Operator = ">", RightValue = (object)5L },
                new { Input = "5 < 5;", LeftValue = (object)5L, Operator = "<", RightValue = (object)5L },
                new { Input = "5 == 5;", LeftValue = (object)5L, Operator = "==", RightValue = (object)5L },
                new { Input = "5 != 5;", LeftValue = (object)5L, Operator = "!=", RightValue = (object)5L },
                new { Input = "true == true", LeftValue = (object)true, Operator = "==", RightValue = (object)true },
                new { Input = "true != false", LeftValue = (object)true, Operator = "!=", RightValue = (object)false },
                new { Input = "false == false", LeftValue = (object)false, Operator = "==", RightValue = (object)false }
            };

            foreach (var test in tests)
            {
                var lexer = new Lexer(test.Input);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                CheckParserErrors(parser);
                Assert.AreEqual(1, program.Statements.Length);

                var statement = program.Statements[0] as ExpressionStatement;
                Assert.IsNotNull(statement);

                var expression = statement.Expression as InfixExpression;
                Assert.IsNotNull(expression);

                TestInfixExpression(expression, test.LeftValue, test.Operator, test.RightValue);
            }
        }
        
        [Test]
        public void Parser_OperatorPrecedence()
        {
            var tests = new Dictionary<string, string>
            {
                {"-a * b", "((-a) * b)"},
                { "!-a", "(!(-a))"},
                { "a + b + c", "((a + b) + c)"},
                { "a + b - c", "((a + b) - c)"},
                { "a * b * c", "((a * b) * c)"},
                { "a * b / c", "((a * b) / c)"},
                { "a + b / c", "(a + (b / c))"},
                { "a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)"},
                { "3 + 4; -5 * 5", "(3 + 4)((-5) * 5)"},
                { "5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))"},
                { "5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))"},
                { "3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"},
                {"true", "true"},
                {"false", "false"},
                {"3 > 5 == false", "((3 > 5) == false)"},
                {"3 < 5 == true", "((3 < 5) == true)"},
                {"1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)"},
                {"(5 + 5) * 2", "((5 + 5) * 2)"},
                {"2 / (5 + 5)", "(2 / (5 + 5))"},
                {"-(5 + 5)", "(-(5 + 5))"},
                {"!(true == true)", "(!(true == true))"},
                //{"a + add(b * c) + d", "((a + add((b * c))) + d)"},
                //{"add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))", "add(a, b, 1, (2 * 3), (4 + 5), add(6, (7 * 8)))"},
                //{"add(a + b + c * d / f + g)", "add((((a + b) + ((c * d) / f)) + g))"},
                //{"a * [1, 2, 3, 4][b * c] * d", "((a * ([1, 2, 3, 4][(b * c)])) * d)"},
                //{"add(a * b[2], b[1], 2 * [1, 2][1])", "add((a * (b[2])), (b[1]), (2 * ([1, 2][1])))"}
            };

            foreach (var test in tests)
            {
                var lexer = new Lexer(test.Key);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                CheckParserErrors(parser);

                var actual = program.ToString().ToLower();
                Assert.AreEqual(test.Value, actual);
            }
        }
        
        [Test]
        public void Parser_BooleanExpressions()
        {
            var tests = new Dictionary<string, bool> {
                { "true;", true },
                { "false;", false },
            };

            foreach (var test in tests)
            {
                var lexer = new Lexer(test.Key);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                CheckParserErrors(parser);
                Assert.AreEqual(1, program.Statements.Length);

                var statement = program.Statements[0] as ExpressionStatement;
                Assert.IsNotNull(statement);

                var expression = statement.Expression as Boolean;
                Assert.IsNotNull(expression);
                Assert.AreEqual(test.Value, expression.Value);
                Assert.AreEqual(test.Value.ToString().ToLower(), expression.TokenLiteral);
            }
        }

        [Test]
        public void Parser_IfExpression()
        {
            var input = "if (x < y) { x }";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);
            Assert.AreEqual(1, program.Statements.Length);

            var statement = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(statement);
            
            var expression = statement.Expression as IfExpression;
            Assert.IsNotNull(expression);

            TestInfixExpression(expression.Condition, "x", "<", "y");

            Assert.AreEqual(1, expression.Consequence.Statements.Length);

            var consequence = expression.Consequence.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(consequence);

            TestIdentifier(consequence.Expression, "x");

            Assert.IsNull(expression.Alternative);
        }

        [Test]
        public void Parser_IfElseExpression()
        {
            var input = "if (x < y) { x } else { y }";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);
            Assert.AreEqual(1, program.Statements.Length);

            var statement = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(statement);

            var expression = statement.Expression as IfExpression;
            Assert.IsNotNull(expression);

            TestInfixExpression(expression.Condition, "x", "<", "y");

            Assert.AreEqual(1, expression.Consequence.Statements.Length);
            var consequence = expression.Consequence.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(consequence);
            TestIdentifier(consequence.Expression, "x");

            Assert.AreEqual(1, expression.Alternative.Statements.Length);
            var alternative = expression.Alternative.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(alternative);
            TestIdentifier(alternative.Expression, "y");
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
            if (value is Int64)
                TestIntegerLiteral(expression, (Int64)value);
            else if (value is string)
                TestIdentifier(expression, (string)value);
            else if (value is bool)
                TestBooleanLiteral(expression, (bool)value);
            else
                Assert.Fail("Invalid type");
        }

        private void TestInfixExpression(Expression expression, object left, string @operator, object right)
        {
            var infixExpression = expression as InfixExpression;
            Assert.IsNotNull(infixExpression);

            TestLiteralExpression(infixExpression.Left, left);
            Assert.AreEqual(@operator, infixExpression.Operator);

            TestLiteralExpression(infixExpression.Right, right);
        }

        private void TestIdentifier(Expression expression, string value)
        {
            var identifier = expression as Identifier;
            Assert.NotNull(identifier);
            Assert.AreEqual(identifier.Value, value);
            Assert.AreEqual(value, identifier.TokenLiteral);
        }

        private void TestIntegerLiteral(Expression expression, Int64 value)
        {
            var integerLiteral = expression as IntegerLiteral;
            Assert.NotNull(integerLiteral);
            Assert.AreEqual(integerLiteral.Value, value);
            Assert.AreEqual($"{value}", integerLiteral.TokenLiteral);
        }

        private void TestBooleanLiteral(Expression expression, bool value)
        {
            var boolean = expression as Boolean;
            Assert.NotNull(boolean);
            Assert.AreEqual(boolean.Value, value);
            Assert.AreEqual(value.ToString().ToLower(), boolean.TokenLiteral);
        }

    }
}
