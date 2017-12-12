using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using BooInterpreter.Objects;
using Boolean = BooInterpreter.Objects.Boolean;

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
            var tests = new Dictionary<string, long?> {
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

        private object TestEval(string input)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            var evaluator = new Evaluator();
            return evaluator.Eval(program);
        }

        private void TestIntegerObject(object obj, Int64 expected)
        {
            var actual = obj as Integer;
            Assert.IsNotNull(actual);;
            Assert.AreEqual(expected, actual.Value);
        }

        private void TestBooleanObject(Object obj, bool expected)
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
