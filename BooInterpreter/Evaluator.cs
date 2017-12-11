using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class Evaluator
    {
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

            else if (node is IntegerLiteral literal)
                return literal.Value;

            return null;
        }

        private object EvalStatements(Statement[] statements)
        {
            object result = null;

            foreach (var statement in statements)
                result = Eval(statement);

            return result;
        }
    }
}
