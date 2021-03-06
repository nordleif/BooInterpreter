﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class Program : Node
    {
        public Program()
        {

        }

        public Token Token { get; set; }

        public Statement[] Statements { get; set; }

        public string TokenLiteral => Statements?.FirstOrDefault()?.TokenLiteral ?? string.Empty;

        public override string ToString()
        {
            return string.Join("", Statements?.Select(s => s.ToString()));
        }
    }
}
