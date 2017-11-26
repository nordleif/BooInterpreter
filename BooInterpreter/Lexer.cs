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
        private int m_position;     // current position in input (points to current char)
        private int m_readPosition; // current readiong position in input (after current char)
        private char m_ch;          // current char under examination

        public Lexer(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input));

            m_input = input;
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

                case (char)0:
                    token = new Token(TokenType.EOF, "");
                    break;

                default:
                    if (IsLetter(m_ch))
                    {
                        var identifier = ReadIdentifier();
                        return new Token(Token.Lookup(identifier), identifier);
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

        private void SkipWhiteSpace()
        {
            while (m_ch == ' ' || m_ch == '\t' || m_ch == '\n' || m_ch == '\r')
                ReadChar();
        }
    }
}
