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
                return EvalStatements(program.Statements);

            else if (node is ExpressionStatement statement)
                return Eval(statement.Expression);

            else if (node is IntegerLiteral integer)
                return new Integer { Value = integer.Value };

            else if (node is BooleanLiteral boolean)
                return NativeBoolToBooleanObject(boolean.Value);

            else if (node is PrefixExpression prefix)
                return EvalPrefixExpression(prefix.Operator, Eval(prefix.Right));

            return m_null;
        }

        private object EvalStatements(Statement[] statements)
        {
            object result = null;

            foreach (var statement in statements)
                result = Eval(statement);

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

        private Boolean NativeBoolToBooleanObject(bool input)
        {
            return input ? m_true : m_false;
        }
    }
}
