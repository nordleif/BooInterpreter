using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class Lexer
    {
        private string m_input;
        private readonly Dictionary<string, TokenType> m_keywords;
        private int m_position;
        private int m_readPosition;
        private char m_ch;

        public Lexer(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input));

            m_input = input;
            m_keywords = new Dictionary<string, TokenType>
            {
                { "fn", TokenType.FUNCTION },
                { "let", TokenType.LET },
                { "true", TokenType.TRUE },
                { "false", TokenType.FALSE },
                { "if", TokenType.IF },
                { "else", TokenType.ELSE },
                { "return", TokenType.RETURN },
            };

            ReadChar();
        }

        public Token NextToken()
        {
            SkipWhiteSpace();

            Token token;
            switch(m_ch)
            {
                case '=':
                    if (PeekChar() == '=')
                    {
                        var ch = m_ch;
                        ReadChar();
                        token = new Token(TokenType.EQ, new string(new char[] { ch, m_ch }));
                    }
                    else
                        token = new Token(TokenType.ASSIGN, m_ch);
                    break;
                case '+':
                    token = new Token(TokenType.PLUS, m_ch);
                    break;
                case '-':
                    token = new Token(TokenType.MINUS, m_ch);
                    break;
                case '!':
                    if (PeekChar() == '=')
                    {
                        var ch = m_ch;
                        ReadChar();
                        token = new Token(TokenType.NOT_EQ, new string(new char[] { ch, m_ch }));
                    }
                    else
                        token = new Token(TokenType.BANG, m_ch);
                    break;
                case '/':
                    token = new Token(TokenType.SLASH, m_ch);
                    break;
                case '*':
                    token = new Token(TokenType.ASTERISK, m_ch);
                    break;
                case '<':
                    token = new Token(TokenType.LT, m_ch);
                    break;
                case '>':
                    token = new Token(TokenType.GT, m_ch);
                    break;
                case ';':
                    token = new Token(TokenType.SEMICOLON, m_ch);
                    break;
                case ',':
                    token = new Token(TokenType.COMMA, m_ch);
                    break;
                case '(':
                    token = new Token(TokenType.LPAREN, m_ch);
                    break;
                case ')':
                    token = new Token(TokenType.RPAREN, m_ch);
                    break;
                case '{':
                    token = new Token(TokenType.LBRACE, m_ch);
                    break;
                case '}':
                    token = new Token(TokenType.RBRACE, m_ch);
                    break;
                case '"':
                    token = new Token(TokenType.STRING, ReadString());
                    break;
                case (char)0:
                    token = new Token(TokenType.EOF, "");
                    break;

                default:
                    if (IsLetter(m_ch))
                    {
                        var identifier = ReadIdentifier();
                        if (!m_keywords.TryGetValue(identifier, out TokenType type))
                            type = TokenType.IDENT;
                        return new Token(type, identifier);
                    }
                    else if (IsDigit(m_ch))
                        return new Token(TokenType.INT, ReadNumber());
                    else
                        token = new Token(TokenType.ILLEGAL, m_ch);
                    break;
            }
            ReadChar();
            return token;
        }

        private bool IsDigit(char ch)
        {
            return '0' <= ch && ch <= '9';
        }

        private bool IsLetter(char ch)
        {
            var result =  'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_';
            return result;
        }

        private char PeekChar()
        {
            if (m_readPosition >= m_input.Length)
                return (char)0;
            else
               return m_input[m_readPosition];
        }

        private void ReadChar()
        {
            if (m_readPosition >= m_input.Length)
                m_ch = (char)0;
            else
                m_ch = m_input[m_readPosition];

            m_position = m_readPosition;
            m_readPosition += 1;
        }

        private string ReadIdentifier()
        {
            var positon = m_position;
            while (IsLetter(m_ch))
                ReadChar();
            return m_input.Substring(positon, m_position - positon);
        }

        private string ReadNumber()
        {
            var position = m_position;
            while (IsDigit(m_ch))
                ReadChar();
            return m_input.Substring(position, m_position - position);
        }

        private string ReadString()
        {
            var position = m_position + 1;
            while(true)
            {
                ReadChar();
                if (m_ch == '"' || m_ch == (char)0)
                    break;
            }
            return m_input.Substring(position, m_position - position);
        }

        private void SkipWhiteSpace()
        {
            while (m_ch == ' ' || m_ch == '\t' || m_ch == '\n' || m_ch == '\r')
                ReadChar();
        }
    }
}
