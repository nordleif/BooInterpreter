using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using BooInterpreter.Objects;
using Array = BooInterpreter.Objects.Array;
using Boolean = BooInterpreter.Objects.Boolean;
using Object = BooInterpreter.Objects.Object;
using String = BooInterpreter.Objects.String;

namespace BooInterpreter
{
    public class EvaluatorTests
    {
        [Test]
        public void Evaluator_EvalIntegerExpressions()
        {
            var tests = new Dictionary<string, Int64> {
                {"5", 5},
                {"10", 10},
                {"-5", -5},
                {"-10", -10},
                {"5 + 5 + 5 + 5 - 10", 10},
                {"2 * 2 * 2 * 2 * 2", 32},
                {"-50 + 100 + -50", 0},
                {"5 * 2 + 10", 20},
                {"5 + 2 * 10", 25},
                {"20 + 2 * -10", 0},
                {"50 / 2 * 2 + 10", 60},
                {"2 * (5 + 10)", 30},
                {"3 * 3 * 3 + 10", 37},
                {"3 * (3 * 3) + 10", 37},
                {"(5 + 10 * 2 + 15 / 3) * 2 + -10", 50}
            };

            foreach (var test in tests)
            {
                var evaluated = TestEval(test.Key);
                TestIntegerObject(evaluated, test.Value);
            }
        }

        [Test]
        public void Evaluator_EvalBooleanExpressions()
        {
            var tests = new Dictionary<string, bool> {
                {"true", true},
                {"false", false},
                {"1 > 2", false},
                {"1 < 2", true},
                {"1 < 1", false},
                {"1 > 1", false},
                {"1 == 1", true},
                {"1 != 1", false},
                {"1 == 2", false},
                {"1 != 2", true},
                { "true == true", true},
                {"false == false", true},
                {"true == false", false},
                {"true != false", true},
                {"false != true", true},
                {"(1 < 2) == true", true},
                {"(1 < 2) == false", false},
                {"(1 > 2) == true", false},
                {"(1 > 2) == false", true}
            };

            foreach (var test in tests)
            {
                var evaluated = TestEval(test.Key);
                TestBooleanObject(evaluated, test.Value);
            }
        }

        [Test]
        public void Evaluator_EvalBangOperator()
        {
            var tests = new Dictionary<string, bool> {
                {"!true", false},
                {"!false", true},
                {"!5", false},
                {"!!true", true},
                {"!!false", false},
                {"!!5", true}
            };

            foreach (var test in tests)
            {
                var evaluated = TestEval(test.Key);
                TestBooleanObject(evaluated, test.Value);
            }
        }

        [Test]
        public void Evaluator_EvalIfElseExpressions()
        {
            var tests = new Dictionary<string, Int64?> {
                {"if (true) { 10 }", 10},
                {"if (false) { 10 }", null},
                {"if (1) { 10 }", 10},
                {"if (1 < 2) { 10 }", 10},
                {"if (1 > 2) { 10 }", null},
                {"if (1 > 2) { 10 } else { 20 }", 20},
                {"if (1 < 2) { 10 } else { 20 }", 10}
            };

            foreach (var test in tests)
            {
                var evaluated = TestEval(test.Key);
                if (test.Value.HasValue)
                    TestIntegerObject(evaluated, test.Value.Value);
                else
                    TestNullObject(evaluated);
            }
        }

        [Test]
        public void Evaluator_EvalReturnStatements()
        {
            var tests = new Dictionary<string, Int64> {
                {"return 10;", 10},
                {"return 10; 9;", 10},
                {"return 2 * 5; 9;", 10},
                {"9; return 2 * 5; 9;", 10},
                {"if (10 > 1) { return 10; }", 10},
                {@" if (10 > 1)
                    {
                        if (10 > 1)
                        {
                            return 10;
                        }
                        return 1;
                    }", 10
                },
                //{@" let f = fn(x) {
                //        return x;
                //        x + 10;
                //    };
                //    f(10);", 10
                //},
                //{@" let f = fn(x) {
                //        let result = x + 10;
                //        return result;
                //        return 10;
                //    };
                //    f(10);", 20
                //}
            };

            foreach (var test in tests)
            {
                var evaluated = TestEval(test.Key);
                TestIntegerObject(evaluated, test.Value);
            }
        }

        [Test]
        public void Evaluator_ErrorHandling()
        {
            var tests = new Dictionary<string, string> {
                    {
                        "5 + true;",
                        "type mismatch: Integer + Boolean"
                    },
                    {
                        "5 + true; 5;",
                        "type mismatch: Integer + Boolean"
                    },
                    {
                        "-true",
                        "unknown operator: -Boolean"
                    },
                    {
                        "true + false;",
                        "unknown operator: Boolean + Boolean"
                    },
                    {
                        "5; true + false; 5",
                        "unknown operator: Boolean + Boolean"
                    },
                    {
                        "if (10 > 1) { true + false; }",
                        "unknown operator: Boolean + Boolean"
                    },
                    {
                        @"if (10 > 1) {
                           if (10 > 1) {
                              return true + false;
                           }
                           return 1;
                        }
                    ", "unknown operator: Boolean + Boolean"
                    },
                    {
                        "foobar",
                        "identifier not found: foobar"
                    },
                    {
                        "\"Hello\" - \"World\"",
                        "unknown operator: String - String"
                    },
                    //{
                    //    "{\"name\": \"Monkey\"}[fn(x) { x }];",
                    //    "unusable as hash key: FUNCTION"
                    //}
            };

            foreach (var test in tests)
            {
                var evaluated = TestEval(test.Key);

                var error = evaluated as Error;
                Assert.IsNotNull(error);
                Assert.AreEqual(test.Value, error.Message);
            }
        }

        [Test]
        public void Evaluator_LetStatements()
        {
            var tests = new Dictionary<string, long> {
                {"let a = 5; a;", 5},
                {"let a = 5 * 5; a;", 25},
                {"let a = 5; let b = a; b;", 5},
                {"let a = 5; let b = a; let c = a + b + 5; c;", 15}
            };

            foreach (var test in tests)
            {
                TestIntegerObject(TestEval(test.Key), test.Value);
            }
        }

        [Test]
        public void Evaluator_FunctionObject()
        {
            var input = "fn(x) { x + 2; };";
            var evaluated = TestEval(input);
            var function = evaluated as Function;

            Assert.IsNotNull(function);
            Assert.AreEqual("x", function.Parameters[0].ToString());
            Assert.AreEqual("(x + 2)", function.Body.ToString());
        }

        [Test]
        public void Evaluator_FunctionApplication()
        {
            var tests = new Dictionary<string, long>
            {
                {"let identity = fn(x) { x; }; identity(5);", 5},
                {"let identity = fn(x) { return x; }; identity(5);", 5},
                {"let double = fn(x) { x * 2; }; double(5);", 10},
                {"let add = fn(x, y) { x + y; }; add(5, 5);", 10},
                {"let add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20},
                {"fn(x) { x; }(5)", 5}
            };

            foreach (var test in tests)
            {
                TestIntegerObject(TestEval(test.Key), test.Value);
            }
        }

        [Test]
        public void Evaluator_Closures()
        {
            var input = @"let newAdder = fn(x) {
                             fn(y) { x + y };
                          };
                          
                          let addTwo = newAdder(2);
                          addTwo(2);

                          ";

            var evaluated = TestEval(input);
            TestIntegerObject(evaluated, 4);
        }

        [Test]
        public void Evaluator_StringLiteral()
        {
            var input = "\"Hello World!\"";
            var evaluated = TestEval(input);
            var str = evaluated as String;

            Assert.IsNotNull(str);
            Assert.AreEqual("Hello World!", str.Value);
        }

        [Test]
        public void Evaluator_StringConcatenation()
        {
            var input = "\"Hello\" + \" \" + \"World!\"";
            var evaluated = TestEval(input);
            var str = evaluated as String;

            Assert.IsNotNull(str);
            Assert.AreEqual("Hello World!", str.Value);
        }

        [Test]
        public void Evaluator_BuiltinFunctions()
        {
            var tests = new Dictionary<string, object>
            {
                {"len(\"\")", 0L},
                {"len(\"four\")", 4L},
                {"len(\"hello world\")", 11L},
                {"len(1)", "argument to 'len' not supported, got Integer"},
                {"len(\"one\", \"two\")", "wrong number of arguments. got=2, want=1"},
                //{"len([1, 2, 3])", 3},
                //{"len([])", 0},
                //{"first([1, 2, 3])", 1},
                //{"first([])", null},
                //{"first(1)", "argument to `first` must be ARRAY, got INTEGER"},
                //{"last([1, 2, 3])", 3},
                //{"last([])", null},
                //{"last(1)", "argument to `last` must be ARRAY, got INTEGER"},
                //{"rest([1, 2, 3])", new []{2, 3}},
                //{"rest([])", null},
                //{"push([], 1)", new [] {1}},
                //{"push(1, 1)", "argument to `push` must be ARRAY, got INTEGER"}
            };

            foreach (var test in tests)
            {
                var evaluated = TestEval(test.Key);
                if (test.Value is Int64 expected)
                {
                    TestIntegerObject(evaluated, expected);
                }
                if (test.Value is string)
                {
                    var error = evaluated as Error;
                    Assert.IsNotNull(error);
                    Assert.AreEqual(test.Value, error.Message);
                }
            }
        }

        [Test]
        public void Evaluator_ArrayLiterals()
        {
            var input = "[1, 2 * 2, 3 + 3]";
            var evaluated = TestEval(input);
            var array = evaluated as Array;

            Assert.IsNotNull(array);
            Assert.AreEqual(3, array.Elements.Length);
            TestIntegerObject(array.Elements[0], 1L);
            TestIntegerObject(array.Elements[1], 4L);
            TestIntegerObject(array.Elements[2], 6L);
        }

        [Test]
        public void Evaluator_ArrayIndexExpressions()
        {
            var tests = new Dictionary<string, long?> {
                { "[1, 2, 3][0]", 1 },
                { "[1, 2, 3][1]", 2 },
                { "[1, 2, 3][2]", 3 },
                { "let i = 0; [1][i];", 1 },
                { "[1, 2, 3][1 + 1];", 3 },
                { "let myArray = [1, 2, 3]; myArray[2];", 3 },
                { "let myArray = [1, 2, 3]; myArray[0] + myArray[1] + myArray[2];", 6 },
                { "let myArray = [1, 2, 3]; let i = myArray[0]; myArray[i]", 2 },
                { "[1, 2, 3][3]", null },
                { "[1, 2, 3][-1]", null }
            };

            foreach (var test in tests)
            {
                var evaluated = TestEval(test.Key);
                if (test.Value.HasValue)
                    TestIntegerObject(evaluated, test.Value.Value);
                else
                    TestNullObject(evaluated);
            }
        }

        private Object TestEval(string input)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            var evaluator = new Evaluator();
            var environment = new Environment();
            return evaluator.Eval(program, environment);
        }

        private void TestIntegerObject(object obj, Int64 expected)
        {
            var actual = obj as Integer;
            Assert.IsNotNull(actual); ;
            Assert.AreEqual(expected, actual.Value);
        }

        private void TestBooleanObject(object obj, bool expected)
        {
            var actual = obj as Boolean;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Value);
        }

        private void TestNullObject(object obj)
        {
            Assert.IsInstanceOf<Null>(obj);
        }
    }
}
