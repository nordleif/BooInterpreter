using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class Parser
    {
        private List<string> m_errors;
        private readonly Dictionary<TokenType, Func<Expression, Expression>> m_infixParse = new Dictionary<TokenType, Func<Expression, Expression>>();
        private Lexer m_lexer;
        private readonly Dictionary<TokenType, Func<Expression>> m_prefixParse = new Dictionary<TokenType, Func<Expression>>();

        public Parser(Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            m_lexer = lexer;
            m_errors = new List<string>();

            m_prefixParse.Add(TokenType.IDENT, ParseIdentifier);
            m_prefixParse.Add(TokenType.INT, ParseIntegerLiteral);

            NextToken();
            NextToken();
        }

        public Token CurrentToken { get; set; }

        public string[] Errors => m_errors.ToArray();

        public Token PeekToken { get; set; }

        public void NextToken()
        {
            CurrentToken = PeekToken;
            PeekToken = m_lexer.NextToken();
        }

        public Program ParseProgram()
        {
            var statements = new List<Statement>();
            while (CurrentToken.Type != TokenType.EOF)
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
            switch (CurrentToken.Type)
            {
                case TokenType.LET:
                    return ParseLetStatement();
                case TokenType.RETURN:
                    return ParseReturnStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private LetStatement ParseLetStatement()
        {
            var statement = new LetStatement { Token = CurrentToken };

            if (!ExpectPeek(TokenType.IDENT))
                return null;

            statement.Name = new Identifier { Token = CurrentToken, Value = CurrentToken.Literal };

            if (!ExpectPeek(TokenType.ASSIGN))
                return null;

            NextToken();

            statement.Value = ParseExpression(Precedence.Lowest);

            if (PeekTokenIs(TokenType.SEMICOLON))
                NextToken();

            return statement;
        }

        private ReturnStatement ParseReturnStatement()
        {
            var statement = new ReturnStatement { Token = CurrentToken };

            NextToken();

            statement.ReturnValue = ParseExpression(Precedence.Lowest);

            if (PeekTokenIs(TokenType.SEMICOLON))
                NextToken();

            return statement;
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            var statement = new ExpressionStatement { Token = CurrentToken };

            statement.Expression = ParseExpression(Precedence.Lowest);

            if (PeekTokenIs(TokenType.SEMICOLON))
                NextToken();

            return statement;
        }

        private Expression ParseExpression(Precedence precedence)
        {
            if (m_prefixParse.TryGetValue(CurrentToken.Type, out var prefix))
                return prefix();
            else
                return null;
        }

        private Identifier ParseIdentifier()
        {
            return new Identifier { Token = CurrentToken, Value = CurrentToken.Literal };
        }

        private IntegerLiteral ParseIntegerLiteral()
        {
            var literal = new IntegerLiteral { Token = CurrentToken };

            if (Int64.TryParse(CurrentToken.Literal, out var result))
            {
                literal.Value = result;
                return literal;
            }

            var msg = $"Could not parse {CurrentToken.Literal} as integer.";
            m_errors.Add(msg);
            return null;
        }

        private bool CurrentTokenIs(TokenType type)
        {
            return CurrentToken.Type == type;
        }

        private bool ExpectPeek(TokenType type)
        {
            if (PeekTokenIs(type))
            {
                NextToken();
                return true;
            }
            else
            {
                PeekError(type);
                return false;
            }
        }

        private void PeekError(TokenType tokenType)
        {
            m_errors.Add($"Expected next token to be {tokenType}, got {PeekToken.Type} instead.");
        }

        private bool PeekTokenIs(TokenType type)
        {
            return PeekToken.Type == type;
        }

    }
}