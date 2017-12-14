using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooInterpreter.Objects;
using Object = BooInterpreter.Objects.Object;
using Boolean = BooInterpreter.Objects.Boolean;

namespace BooInterpreter
{
    public class Evaluator
    {
        #region Static Members

        private static readonly Null m_null = new Null { };
        private static readonly Boolean m_false = new Boolean { Value = false };
        private static readonly Boolean m_true = new Boolean { Value = true };

        #endregion

        public Evaluator()
        {

        }

        public Object Eval(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            else if (node is Program program)
                return EvalProgram(program);

            else if (node is BlockStatement block)
                return EvalBlockStatement(block);

            else if (node is ExpressionStatement statement)
                return Eval(statement.Expression);

            else if (node is IntegerLiteral integer)
                return new Integer { Value = integer.Value };

            else if (node is BooleanLiteral boolean)
                return NativeBoolToBooleanObject(boolean.Value);

            else if (node is PrefixExpression prefix)
            {
                var value = Eval(prefix.Right);
                if (value is Error)
                    return value;
                return EvalPrefixExpression(prefix.Operator, value);
            }

            else if (node is InfixExpression infix)
            {
                var left = Eval(infix.Left);
                if (left is Error)
                    return left;

                var right = Eval(infix.Right);
                if (right is Error)
                    return right;

                return EvalInfixExpression(infix.Operator, left, right);
            }
            
            else if (node is IfExpression ifExpression)
                return EvalIfExpression(ifExpression);

            else if (node is ReturnStatement returnStatement)
            {
                var value = Eval(returnStatement.ReturnValue);
                if (value is Error)
                    return value;
                return new ReturnValue { Value = value };
            }

            return m_null;
        }

        private Object EvalProgram(Program program)
        {
            Object result = null;

            foreach (var statement in program.Statements)
            {
                result = Eval(statement);

                if (result is ReturnValue returnValue)
                    return returnValue.Value;
                else if (result is Error error)
                    return error;
            }

            return result;
        }

        private Object EvalBlockStatement(BlockStatement block)
        {
            Object result = null;

            foreach(var statement in block.Statements)
            {
                result = Eval(statement);
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

        private Object EvalIfExpression(IfExpression ifExpression)
        {
            var condition = Eval(ifExpression.Condition);
            if (condition is Error)
                return condition;

            if (IsTruthy(condition))
                return Eval(ifExpression.Consequence);
            else if (ifExpression.Alternative != null)
                return Eval(ifExpression.Alternative);
            else
                return m_null;
        }

        private Object NativeBoolToBooleanObject(bool input)
        {
            return input ? m_true : m_false;
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
