using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class Parser
    {
        private Token m_currentToken;
        private Token m_peekToken;
        private Lexer m_lexer;

        public Parser(Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            m_lexer = lexer;

            NextToken();
            NextToken();
        }

        public void NextToken()
        {
            m_currentToken = m_peekToken;
            m_peekToken = m_lexer.NextToken();
        }

        public Program ParseProgram()
        {
            var statements = new List<Statement>();
            while(m_currentToken.Type != TokenType.EOF)
            {
                var statement = ParseStatement();
                if (statement != null)
                    statements.Add(statement);
                NextToken();
            }
            
            return new Program { Statements = statements.ToArray() };
        }

        private Statement ParseStatement()
        {
            switch(m_currentToken.Type)
            {
                case TokenType.LET:
                    return ParseLetStatement();
                default:
                    return null;
            }
        }

        private LetStatement ParseLetStatement()
        {
            var statement = new LetStatement { Token = m_currentToken };

            if (!ExpectPeek(TokenType.IDENT))
                return null;

            statement.Name = new Identifier { Token = m_currentToken, Value = m_currentToken.Literal };

            if (!ExpectPeek(TokenType.ASSIGN))
                return null;

            while (CurrentTokenIs(TokenType.SEMICOLON))
                NextToken();

            return statement;        
        }
        
        
        private bool CurrentTokenIs(TokenType type)
        {
            return m_currentToken.Type == type;
        }

        private bool ExpectPeek(TokenType type)
        {
            if (PeekTokenIs(type))
            {
                NextToken();
                return true;
            }
            else
                return false;
        }

        private bool PeekTokenIs(TokenType type)
        {
            return m_peekToken.Type == type;
        }
        
    }
}
