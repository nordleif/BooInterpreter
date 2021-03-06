﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class CallExpression : Expression
    {
        public CallExpression()
        {

        }

        public Expression Function { get; set; }

        public Expression[] Arguments { get; set; }

        public override string ToString()
        {
            var text = $"{Function}";
            if (Arguments != null)
                text += $"({string.Join(", ", Arguments?.Select(a => a.ToString()))})";
            return text;
        }
    }
}
