﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public enum ObjectType
    {
        Null,
        Boolean,
        Integer,
        ReturnValue,
        Error,
        Function,
        String,
        Builtin,
        Array,
        Hash,
    }

    public enum Precedence
    {
        Lowest = 0,
        Assign,      // =
        Equals,      // ==
        LessGreater, // > or <
        Sum,         // +
        Product,     // *
        Prefix,      // -X or !X
        Call,        // myFunction(X)
        Index,       // array[index]
    }
    
    public enum TokenType
    {
        ILLEGAL,
        EOF,

        // Identifiers + literals
        IDENT, // add, foobar, x, y, ...
        INT, // 1343456
        STRING, // "foobar"

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
        COLON,

        LPAREN,
        RPAREN,
        LBRACE,
        RBRACE,
        LBRACKET,
        RBRACKET,

        // Keywords
        FUNCTION,
        LET,
        TRUE,
        FALSE,
        IF,
        ELSE,
        RETURN,
        WHILE,

        EQ,
        NOT_EQ,

    }
}
