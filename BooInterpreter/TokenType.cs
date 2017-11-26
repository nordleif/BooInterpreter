using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public enum TokenType 
    {
        ILLEGAL,
        EOF,
        
        // Identifiers + literals
        IDENT, // add, foobar, x, y, ...
        INT, // 1343456

        // Operators
        ASSIGN,
        PLUS,
        MINUS,
        BANG,
        ASTERISK,
        SLASH,

        LT,
        GT,

        // Delimiters
        COMMA,
        SEMICOLON,

        LPAREN,
        RPAREN,
        LBRACE,
        RBRACE,

        // Keywords
        FUNCTION,
        LET,
        TRUE,
        FALSE,
        IF,
        ELSE,
        RETURN,

        EQ,
        NOT_EQ,
    }
}
