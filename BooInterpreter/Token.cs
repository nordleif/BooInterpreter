using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class Token
    {
        private static readonly Dictionary<string, TokenType> m_keywords = new Dictionary<string, TokenType>
        {
            { "fn", TokenType.FUNCTION },
            { "let", TokenType.LET },
            { "true", TokenType.TRUE },
            { "false", TokenType.FALSE },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "return", TokenType.RETURN },
        };

        public static TokenType Lookup(string literal)
        {
            if (m_keywords.TryGetValue(literal, out TokenType type))
                return type;
            else
                return TokenType.IDENT;
        }

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
    }
}
