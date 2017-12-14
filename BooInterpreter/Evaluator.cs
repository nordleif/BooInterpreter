using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooInterpreter.Objects;
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

        public object Eval(Node node)
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
                return EvalPrefixExpression(prefix.Operator, Eval(prefix.Right));

            else if (node is InfixExpression infix)
                return EvalInfixExpression(infix.Operator, Eval(infix.Left), Eval(infix.Right));
            
            else if (node is IfExpression ifExpression)
                return EvalIfExpression(ifExpression);

            else if (node is ReturnStatement returnStatement)
                return new ReturnValue { Value = Eval(returnStatement.ReturnValue) };

            return m_null;
        }

        private object EvalProgram(Program program)
        {
            object result = null;

            foreach (var statement in program.Statements)
            {
                result = Eval(statement);
                if (result is ReturnValue returnValue)
                    return returnValue.Value;
            }

            return result;
        }

        private object EvalBlockStatement(BlockStatement block)
        {
            object result = null;

            foreach(var statement in block.Statements)
            {
                result = Eval(statement);
                if (result is ReturnValue returnValue)
                    return returnValue;
            }
            
            return result;
        }
        
        private object EvalPrefixExpression(string op, object right)
        {
            switch(op)
            {
                case "!":
                    return EvalBangOperatorExpression(right);
                case "-":
                    return EvalMinusPrefixOperatorExpression(right);
                default:
                    return m_null;
            }
        }

        private object EvalBangOperatorExpression(object right)
        {
            if (right is Boolean boolean)
                return boolean.Equals(m_true) ? m_false : m_true;
            else if (right is Null)
                return m_true;
            else
                return m_false;
        }

        private object EvalMinusPrefixOperatorExpression(object right)
        {
            if (right is Integer integer)
                return new Integer { Value = -integer.Value };
            else
                return m_null;

        }

        private object EvalInfixExpression(string op, object left, object right)
        {
            if (left is Integer && right is Integer)
                return EvalIntegerInfixExpression(op, (Integer)left, (Integer)right);
         
            switch(op)
            {
                case "==":
                    return NativeBoolToBooleanObject(left == right);
                case "!=":
                    return NativeBoolToBooleanObject(left != right);
            }
            
            return m_null;
        }

        private object EvalIntegerInfixExpression(string op, Integer left, Integer right)
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
                    return m_null;
            }
        }

        private object EvalIfExpression(IfExpression ifExpression)
        {
            var condition = Eval(ifExpression.Condition);

            if (IsTruthy(condition))
                return Eval(ifExpression.Consequence);
            else if (ifExpression.Alternative != null)
                return Eval(ifExpression.Alternative);
            else
                return m_null;
        }
        
        private Boolean NativeBoolToBooleanObject(bool input)
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
