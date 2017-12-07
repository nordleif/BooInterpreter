using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class Token
    {
        public Token(TokenType type, char literal)
        {
            Type = type;
            Literal = new string(literal, 1);
        }

        public Token(TokenType type, string literal)
        {
            Type = type;
            Literal = literal;
        }

        public TokenType Type { get; set; }

        public string Literal { get; set; }

        public override string ToString()
        {
            return Literal;
        }
    }
}
