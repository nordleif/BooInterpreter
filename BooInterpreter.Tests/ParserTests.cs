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
                {"a + add(b * c) + d", "((a + add((b * c))) + d)"},
                {"add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))", "add(a, b, 1, (2 * 3), (4 + 5), add(6, (7 * 8)))"},
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
        public void Parser_BooleanLiteralExpressions()
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

                var expression = statement.Expression as BooleanLiteral;
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

        [Test]
        public void Parser_FunctionLiteral()
        {
            var input = "fn(x, y) { x + y; }";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);
            Assert.AreEqual(1, program.Statements.Length);

            var statement = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(statement);

            var function = statement.Expression as FunctionLiteral;
            Assert.IsNotNull(function);
            Assert.AreEqual(2, function.Parameters.Length);

            TestLiteralExpression(function.Parameters[0], "x");
            TestLiteralExpression(function.Parameters[1], "y");

            Assert.AreEqual(1, function.Body.Statements.Length);
            var bodyStatement = function.Body.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(bodyStatement);
            TestInfixExpression(bodyStatement.Expression, "x", "+", "y");
        }

        [Test]
        public void Parser_FunctionParameters()
        {
            var tests = new[] {
                new { Input = "fn() {};", ExpectedParams = new string[0] },
                new { Input = "fn(x) {};", ExpectedParams = new [] {"x"} },
                new { Input = "fn(x, y, z) {};",  ExpectedParams = new [] {"x", "y", "z"} }
            };

            foreach (var test in tests)
            {
                var lexer = new Lexer(test.Input);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                CheckParserErrors(parser);

                var statement = program.Statements[0] as ExpressionStatement;
                Assert.IsNotNull(statement);

                var function = statement.Expression as FunctionLiteral;
                Assert.AreEqual(test.ExpectedParams.Length, function.Parameters.Length);

                for (var i = 0; i < test.ExpectedParams.Length; i++)
                {
                    var ident = test.ExpectedParams[i];
                    TestLiteralExpression(function.Parameters[i], ident);
                }
            }
        }

        [Test]
        public void Parser_CallExpression()
        {
            var input = "add(1, 2 * 3, 4 + 5);";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);
            Assert.AreEqual(1, program.Statements.Length);

            var statement = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(statement);

            var expression = statement.Expression as CallExpression;
            Assert.IsNotNull(expression);

            TestIdentifier(expression.Function, "add");

            Assert.AreEqual(3, expression.Arguments.Length);
            TestLiteralExpression(expression.Arguments[0], 1L);
            TestInfixExpression(expression.Arguments[1], 2L, "*", 3L);
            TestInfixExpression(expression.Arguments[2], 4L, "+", 5L);
        }

        [Test]
        public void Parser_StringLiteralExpression()
        {
            var input = "\"hello world\";";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);
            Assert.AreEqual(1, program.Statements.Length);

            var statement = program.Statements.First() as ExpressionStatement;
            Assert.IsNotNull(statement);
            Assert.IsNotNull(statement.Expression);

            var expression = statement.Expression as StringLiteral;
            Assert.IsNotNull(expression);
            Assert.AreEqual("hello world", expression.Value);
            Assert.AreEqual("hello world", expression.TokenLiteral);
        }

        [Test]
        public void Parser_ArrayLiterals()
        {
            var input = "[1, 2 * 2, 3 + 3]";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);

            var statement = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(statement);
            
            var array = statement.Expression as ArrayLiteral;
            Assert.IsNotNull(array);
            Assert.AreEqual(3, array.Elements.Length);
            TestIntegerLiteral(array.Elements[0], 1);
            TestInfixExpression(array.Elements[1], 2L, "*", 2L);
            TestInfixExpression(array.Elements[2], 3L, "+", 3L);
        }

        [Test]
        public void Parser_IndexExpressions()
        {
            var input = "myArray[1 + 1]";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);

            var statement = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(statement);

            var indexExpression = statement.Expression as IndexExpression;
            Assert.IsNotNull(indexExpression);
            TestIdentifier(indexExpression.Left, "myArray");
            TestInfixExpression(indexExpression.Index, 1L, "+", 1L);
        }

        [Test]
        public void Parser_HashLiteralsStringKeys()
        {
            var input = "{\"one\": 1, \"two\": 2, \"three\": 3}";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);

            var statement = program.Statements[0] as ExpressionStatement;
            var hash = statement.Expression as HashLiteral;
            Assert.IsNotNull(hash);

            var expected = new Dictionary<string, Int64>
            {
                { "one", 1L },
                { "two", 2L },
                { "three", 3L }
            };

            foreach (var pair in hash.Pairs)
            {
                var literal = pair.Key as StringLiteral;
                Assert.IsNotNull(literal);

                var expectedValue = expected[literal.Value];
                TestIntegerLiteral(pair.Value, expectedValue);
            }
        }

        [Test]
        public void Parser_HashLiteralsIntegerKeys()
        {
            var input = "{1: 1, 2: 2, 3: 3}";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);

            var statement = program.Statements[0] as ExpressionStatement;
            var hash = statement.Expression as HashLiteral;
            Assert.IsNotNull(hash);

            var expected = new Dictionary<Int64, Int64>
            {
                { 1L, 1L },
                { 2L, 2L },
                { 3L, 3L }
            };

            foreach (var pair in hash.Pairs)
            {
                var literal = pair.Key as IntegerLiteral;
                Assert.IsNotNull(literal);

                var expectedValue = expected[literal.Value];
                TestIntegerLiteral(pair.Value, expectedValue);
            }
        }

        [Test]
        public void Parser_HashLiteralsBooleanKeys()
        {
            var input = "{true: 1, false: 2}";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);

            var statement = program.Statements[0] as ExpressionStatement;
            var hash = statement.Expression as HashLiteral;
            Assert.IsNotNull(hash);

            var expected = new Dictionary<bool, long>
            {
                { true, 1 },
                { false, 2 }
            };

            foreach (var pair in hash.Pairs)
            {
                var literal = pair.Key as BooleanLiteral;
                Assert.IsNotNull(literal);

                var expectedValue = expected[literal.Value];
                TestIntegerLiteral(pair.Value, expectedValue);
            }
        }

        [Test]
        public void Parser_ParsingEmptyHashLiteral()
        {
            var input = "{}";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);

            var statement = program.Statements[0] as ExpressionStatement;
            var hash = statement.Expression as HashLiteral;
            Assert.IsNotNull(hash);
            Assert.AreEqual(0, hash.Pairs.Count);
        }

        [Test]
        public void Parser_ParsingHashLiteralsWithExpressions()
        {
            var input = "{\"one\": 0 + 1, \"two\": 10 - 8, \"three\": 15 / 5}";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);

            var statement = program.Statements[0] as ExpressionStatement;
            var hash = statement.Expression as HashLiteral;
            Assert.IsNotNull(hash);
            Assert.AreEqual(3, hash.Pairs.Count);

            var tests = new Dictionary<string, Action<Expression>>
            {
                { "one", e => TestInfixExpression(e, 0L, "+", 1L) },
                { "two", e => TestInfixExpression(e, 10L, "-", 8L) },
                { "three", e => TestInfixExpression(e, 15L, "/", 5L) },
            };

            foreach (var pair in hash.Pairs)
            {
                var literal = pair.Key as StringLiteral;
                Assert.IsNotNull(literal);
                Assert.IsTrue(tests.ContainsKey(literal.Value));
                var expression = tests[literal.Value];
                expression(pair.Value);
            }
        }

        [Test]
        public void Parser_WhileExpression()
        {
            var input = "while (x < y) { x }";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);
            Assert.AreEqual(1, program.Statements.Length);

            var statement = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(statement);

            var expression = statement.Expression as WhileExpression;
            Assert.IsNotNull(expression);

            TestInfixExpression(expression.Condition, "x", "<", "y");

            Assert.AreEqual(1, expression.Statement.Statements.Length);
            var consequence = expression.Statement.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(consequence);
            TestIdentifier(consequence.Expression, "x");
        }

        [Test]
        public void Parser_AssignExpression()
        {
            var input = "i = 0;";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            CheckParserErrors(parser);
            Assert.AreEqual(1, program.Statements.Length);

            var statement = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(statement);

            var expression = statement.Expression as AssignExpression;
            Assert.IsNotNull(expression);

            TestIdentifier(expression.Name, "i");
            TestLiteralExpression(expression.Value, 0L);
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
            var boolean = expression as BooleanLiteral;
            Assert.NotNull(boolean);
            Assert.AreEqual(boolean.Value, value);
            Assert.AreEqual(value.ToString().ToLower(), boolean.TokenLiteral);
        }

    }
}
