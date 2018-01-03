using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooInterpreter.Objects;
using Array = BooInterpreter.Objects.Array;
using Boolean = BooInterpreter.Objects.Boolean;
using Object = BooInterpreter.Objects.Object;
using String = BooInterpreter.Objects.String;

namespace BooInterpreter
{
    public class Evaluator
    {
        #region Static Members
        
        private static readonly Null m_null = new Null { };
        private static readonly Boolean m_false = new Boolean { Value = false };
        private static readonly Boolean m_true = new Boolean { Value = true };

        #endregion

        private readonly Dictionary<string, Builtin> m_builtins;

        public Evaluator()
        {
            m_builtins = new Dictionary<string, Builtin>()
            {
                {
                    "len", new Builtin
                    {
                        Function = args => 
                        {
                            if (args.Length != 1)
                                return new Error { Message = $"wrong number of arguments. got={args.Length}, want=1" };
                            else if (args[0] is String str)
                                return new Integer { Value = str.Value.Length };
                            else if (args[0] is Array array)
                                return new Integer { Value = array.Elements.Length };
                            else
                                return new Error { Message = $"argument to 'len' not supported, got {args[0].Type}" };
                        }
                    }
                },
                {
                    "first", new Builtin
                    {
                        Function = args =>
                        {
                            if (args.Length != 1)
                                return new Error { Message = $"wrong number of arguments. got={args.Length}, want=1" };

                            var array = args[0] as Array;
                            if (array == null)
                                return new Error { Message = $"argument to 'first' must be ARRAY, got {args[0].Type}" };

                            if (array.Elements == null || !array.Elements.Any())
                                return m_null;

                            return array.Elements.First();
                        }
                    }
                },
                {
                    "last", new Builtin
                    {
                        Function = args =>
                        {
                            if (args.Length != 1)
                                return new Error { Message = $"wrong number of arguments. got={args.Length}, want=1" };

                            var array = args[0] as Array;
                            if (array == null)
                                return new Error { Message = $"argument to 'last' must be ARRAY, got {args[0].Type}" };

                            if (array.Elements == null || !array.Elements.Any())
                                return m_null;

                            return array.Elements.Last();
                        }
                    }
                },
                {
                    "rest", new Builtin
                    {
                        Function = args =>
                        {
                            if (args.Length != 1)
                                return new Error { Message = $"wrong number of arguments. got={args.Length}, want=1" };

                            var array = args[0] as Array;
                            if (array == null)
                                return new Error { Message = $"argument to 'first' must be ARRAY, got {args[0].Type}" };

                            if (array.Elements == null || !array.Elements.Any())
                                return m_null;

                            return new Array { Elements = array.Elements.Skip(1).ToArray() };
                        }
                    }
                },
                {
                    "push", new Builtin
                    {
                        Function = args =>
                        {
                            if (args.Length != 2)
                                return new Error { Message = $"wrong number of arguments. got={args.Length}, want=2" };

                            var array = args[0] as Array;
                            if (array == null)
                                return new Error { Message = $"argument to 'push' must be ARRAY, got {args[0].Type}" };

                            var list = array.Elements?.ToList();
                            if (list == null)
                                list = new List<Object>();

                            list.Add(args[1]);
                            
                            return new Array { Elements = list.ToArray() };
                        }
                    }
                },
                {
                    "puts", new Builtin
                    {
                        Function = args =>
                        {
                            foreach (var arg in args)
                                Console.WriteLine(arg);
                            return m_null;
                        }
                    }
                },
            };
        }

        public Object Eval(Node node, Environment environment)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (environment == null)
                throw new ArgumentNullException(nameof(environment));

            else if (node is Program program)
                return EvalProgram(program, environment);

            else if (node is BlockStatement block)
                return EvalBlockStatement(block, environment);

            else if (node is ExpressionStatement statement)
                return Eval(statement.Expression, environment);

            else if (node is IntegerLiteral integer)
                return new Integer { Value = integer.Value };

            else if (node is BooleanLiteral boolean)
                return NativeBoolToBooleanObject(boolean.Value);

            else if (node is PrefixExpression prefix)
            {
                var value = Eval(prefix.Right, environment);
                if (value is Error)
                    return value;
                return EvalPrefixExpression(prefix.Operator, value);
            }

            else if (node is InfixExpression infix)
            {
                var left = Eval(infix.Left, environment);
                if (left is Error)
                    return left;

                var right = Eval(infix.Right, environment);
                if (right is Error)
                    return right;

                return EvalInfixExpression(infix.Operator, left, right);
            }

            else if (node is IfExpression ifExpression)
                return EvalIfExpression(ifExpression, environment);

            else if (node is ReturnStatement returnStatement)
            {
                var value = Eval(returnStatement.ReturnValue, environment);
                if (value is Error)
                    return value;
                return new ReturnValue { Value = value };
            }

            else if (node is LetStatement let)
            {
                var value = Eval(let.Value, environment);
                if (value is Error)
                    return value;

                environment.Set(let.Name.Value, value);
            }

            else if (node is Identifier identifier)
                return EvalIdentifier(identifier, environment);

            else if (node is FunctionLiteral functionLiteral)
            {
                var parameters = functionLiteral.Parameters;
                var body = functionLiteral.Body;

                return new Function { Parameters = parameters, Body = body, Environment = environment };
            }

            else if (node is CallExpression callExpression)
            {
                var function = Eval(callExpression.Function, environment);
                if (function is Error)
                    return function;

                var arguments = EvalExpressions(callExpression.Arguments, environment);
                if (arguments.Length == 1 && arguments[0] is Error)
                    return arguments[0];

                return ApplyFunction(function, arguments);
            }

            else if (node is StringLiteral str)
                return new String { Value = str.Value };

            else if (node is ArrayLiteral array)
            {
                var elements = EvalExpressions(array.Elements, environment);
                if (elements.Length == 1 && elements[0] is Error)
                    return elements[0];

                return new Array { Elements = elements };
            }

            else if (node is IndexExpression indexExpression)
            {
                var left = Eval(indexExpression.Left, environment);
                if (left is Error)
                    return left;

                var index = Eval(indexExpression.Index, environment);
                if (index is Error)
                    return index;

                return EvalIndexExpression(left, index);
            }

            else if (node is HashLiteral hashLiteral)
                return EvalHashLiteral(hashLiteral, environment);

            else if (node is WhileExpression whileExpression)
                return EvalWhileExpression(whileExpression, environment);

            else if (node is AssignExpression assignExpression)
                return EvalAssignExpression(assignExpression, environment);

            return m_null;
        }

        private Object EvalProgram(Program program, Environment environment)
        {
            Object result = null;

            foreach (var statement in program.Statements)
            {
                result = Eval(statement, environment);

                if (result is ReturnValue returnValue)
                    return returnValue.Value;
                else if (result is Error error)
                    return error;
            }

            return result;
        }

        private Object EvalBlockStatement(BlockStatement block, Environment environment)
        {
            Object result = null;

            foreach(var statement in block.Statements)
            {
                result = Eval(statement, environment);
                if (result is ReturnValue returnValue)
                    return returnValue;
                else if (result is Error error)
                    return error;
            }
            
            return result;
        }
        
        private Object EvalPrefixExpression(string op, Object right)
        {
            switch(op)
            {
                case "!":
                    return EvalBangOperatorExpression(right);
                case "-":
                    return EvalMinusPrefixOperatorExpression(right);
                default:
                    return new Error { Message = $"unknown operator: {op}{right.Type}" };
            }
        }

        private Object EvalBangOperatorExpression(Object right)
        {
            if (right is Boolean boolean)
                return boolean.Equals(m_true) ? m_false : m_true;
            else if (right is Null)
                return m_true;
            else
                return m_false;
        }

        private Object EvalMinusPrefixOperatorExpression(Object right)
        {
            if (right is Integer integer)
                return new Integer { Value = -integer.Value };
            else
                return new Error { Message = $"unknown operator: -{right.Type}" };

        }

        private Object EvalInfixExpression(string op, Object left, Object right)
        {
            if (left.Type != right.Type)
                return new Error { Message = $"type mismatch: {left.Type} {op} {right.Type}" };

            if (left is Integer && right is Integer)
                return EvalIntegerInfixExpression(op, (Integer)left, (Integer)right);
            else if (left is String && right is String)
                return EvalStringInfixExpression(op, (String)left, (String)right);

            switch(op)
            {
                case "==":
                    return NativeBoolToBooleanObject(left == right);
                case "!=":
                    return NativeBoolToBooleanObject(left != right);
            }
            
            return new Error { Message = $"unknown operator: {left.Type} {op} {right.Type}" };
        }

        private Object EvalIntegerInfixExpression(string op, Integer left, Integer right)
        {
            switch(op)
            {
                case "+":
                    return new Integer { Value = left.Value + right.Value };
                case "-":
                    return new Integer { Value = left.Value - right.Value };
                case "*":
                    return new Integer { Value = left.Value * right.Value };
                case "/":
                    return new Integer { Value = left.Value / right.Value };
                case "<":
                    return NativeBoolToBooleanObject(left.Value < right.Value);
                case ">":
                    return NativeBoolToBooleanObject(left.Value > right.Value);
                case "==":
                    return NativeBoolToBooleanObject(left.Value == right.Value);
                case "!=":
                    return NativeBoolToBooleanObject(left.Value != right.Value);
                default:
                    return new Error { Message = $"unknown operator: {left.Type} {op} {right.Type}" };
            }
        }

        private Object EvalStringInfixExpression(string op, String left, String right)
        {
            switch (op)
            {
                case "+":
                    return new String { Value = left.Value + right.Value };
                default:
                    return new Error { Message = $"unknown operator: {left.Type} {op} {right.Type}" };
            }
        }

        private Object EvalIfExpression(IfExpression ifExpression, Environment environment)
        {
            var condition = Eval(ifExpression.Condition, environment);
            if (condition is Error)
                return condition;

            if (IsTruthy(condition))
                return Eval(ifExpression.Consequence, environment);
            else if (ifExpression.Alternative != null)
                return Eval(ifExpression.Alternative, environment);
            else
                return m_null;
        }

        private Object[] EvalExpressions(Expression[] expressions, Environment environment)
        {
            var result = new List<Object>();

            foreach(var expression in expressions)
            {
                var evaluated = Eval(expression, environment);
                if (evaluated is Error)
                    return new Object[] { evaluated };

                result.Add(evaluated);
            }

            return result.ToArray();
        }

        private Object EvalIndexExpression(Object left, Object index)
        {
            if (left is Array array && index is Integer integer)
                return EvalArrayIndexExpression(array, integer);

            else if (left is Hash hash)
                return EvalHashIndexExpression(hash, index);

            return new Error { Message = $"index operator not supported: {left.Type}" };
        }

        private Object EvalArrayIndexExpression(Array array, Integer index)
        {
            if (index.Value < 0 || index.Value >= array.Elements.Length)
                return m_null;
            else
                return array.Elements[index.Value];
        }

        private Object EvalHashIndexExpression(Hash hash, Object index)
        {
            if (!(index is String || index is Integer || index is Boolean))
                return new Error { Message = $"unusable as hash key: {index.Type}" };

            if (hash.Pairs.TryGetValue(index, out var value))
                return value;

            return m_null;
        }

        private Object EvalHashLiteral(HashLiteral hashLiteral, Environment environment)
        {
            var pairs = new Dictionary<Object, Object>(ObjectComparer.Default);

            foreach(var pair in hashLiteral.Pairs)
            {
                var key = Eval(pair.Key, environment);
                if (key is Error)
                    return key;

                var value = Eval(pair.Value, environment);
                if (value is Error)
                    return value;

                pairs.Add(key, value);
            }

            return new Hash { Pairs = pairs };
        }

        private Object EvalWhileExpression(WhileExpression whileExpression, Environment environment)
        {
            Object result = m_null;

            while (true)
            {
                var condition = Eval(whileExpression.Condition, environment);
                if (condition is Error)
                    return condition;

                if (condition != m_true)
                    return result;

                result = Eval(whileExpression.Statement, environment);
                if (result is ReturnValue)
                    return result;
                else if (result is Error)
                    return result;
            }
        }

        private Object NativeBoolToBooleanObject(bool input)
        {
            return input ? m_true : m_false;
        }
        
        private Object ApplyFunction(Object obj, Object[] arguments)
        {
            if (obj is Function function)
            {
                var extendedEnv = ExtendFunctionEnv(function, arguments.ToArray());
                var evaluated = Eval(function.Body, extendedEnv);
                return UnwrapReturnValue(evaluated);
            }
            else if (obj is Builtin builtin)
            {
                return builtin.Function.Invoke(arguments);
            }
            else
            {
                return new Error { Message = $"not a function: {obj.Type}" };
            }
        }

        private Environment ExtendFunctionEnv(Function function, Object[] arguments)
        {
            var environment = new Environment(function.Environment);
            for (var i = 0; i < function.Parameters.Length; i++)
                environment.Set(function.Parameters[i].Value, arguments[i]);
            return environment;
        }

        private Object UnwrapReturnValue(Object obj)
        {
            if (obj is ReturnValue returnValue)
                return returnValue.Value;
            else
                return obj;
        }

        private Object EvalIdentifier(Identifier identifier, Environment environment)
        {
            var value = environment.Get(identifier.Value);
            if (value != null)
                return value;

            if (m_builtins.TryGetValue(identifier.Value, out var builtin))
                return builtin;

            return new Error { Message = $"identifier not found: {identifier.Value}" };
        }

        private Object EvalAssignExpression(AssignExpression assignExpression, Environment environment)
        {
            if (!environment.Contains(assignExpression.Name.Value))
                return new Error { Message = $"identifier not found: {assignExpression.Name.Value}" };

            var value = Eval(assignExpression.Value, environment);
            if (value is Error)
                return value;

            environment.Set(assignExpression.Name.Value, value);

            return value;
        }

        private bool IsTruthy(object obj)
        {
            if (obj.Equals(m_null))
                return false;
            else if (obj.Equals(m_true))
                return true;
            else if (obj.Equals(m_false))
                return false;
            else
                return true;
        }
    }
}
