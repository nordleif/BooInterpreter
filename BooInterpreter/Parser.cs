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
        private readonly Dictionary<TokenType, Func<Expression, Expression>> m_infixParse;
        private Lexer m_lexer;
        private Dictionary<TokenType, Precedence> m_precedences;
        private readonly Dictionary<TokenType, Func<Expression>> m_prefixParse;

        public Parser(Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            m_lexer = lexer;
            m_errors = new List<string>();

            m_prefixParse = new Dictionary<TokenType, Func<Expression>>();
            m_prefixParse.Add(TokenType.IDENT, ParseIdentifier);
            m_prefixParse.Add(TokenType.INT, ParseIntegerLiteral);
            m_prefixParse.Add(TokenType.BANG, ParsePrefixExpression);
            m_prefixParse.Add(TokenType.MINUS, ParsePrefixExpression);
            m_prefixParse.Add(TokenType.TRUE, ParseBoolean);
            m_prefixParse.Add(TokenType.FALSE, ParseBoolean);
            m_prefixParse.Add(TokenType.LPAREN, ParseGroupedExpression);
            
            m_infixParse = new Dictionary<TokenType, Func<Expression, Expression>>();
            m_infixParse.Add(TokenType.PLUS, ParseInfixExpression);
            m_infixParse.Add(TokenType.MINUS, ParseInfixExpression);
            m_infixParse.Add(TokenType.SLASH, ParseInfixExpression);
            m_infixParse.Add(TokenType.ASTERISK, ParseInfixExpression);
            m_infixParse.Add(TokenType.EQ, ParseInfixExpression);
            m_infixParse.Add(TokenType.NOT_EQ, ParseInfixExpression);
            m_infixParse.Add(TokenType.LT, ParseInfixExpression);
            m_infixParse.Add(TokenType.GT, ParseInfixExpression);

            m_precedences = new Dictionary<TokenType, Precedence>();
            m_precedences.Add(TokenType.EQ, Precedence.Equals);
            m_precedences.Add(TokenType.NOT_EQ, Precedence.Equals);
            m_precedences.Add(TokenType.LT, Precedence.LessGreater);
            m_precedences.Add(TokenType.GT, Precedence.LessGreater);
            m_precedences.Add(TokenType.PLUS, Precedence.Sum);
            m_precedences.Add(TokenType.MINUS, Precedence.Sum);
            m_precedences.Add(TokenType.SLASH, Precedence.Product);
            m_precedences.Add(TokenType.ASTERISK, Precedence.Product);

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
            Func<Expression> prefix = null;
            if (!m_prefixParse.TryGetValue(CurrentToken.Type, out prefix))
            {
                NoPrefixError(CurrentToken.Type);
                return null;
            }

            var leftExpression = prefix();

            while(!PeekTokenIs(TokenType.SEMICOLON) && precedence < PeekPrecedence())
            {
                Func<Expression, Expression> infix = null;
                if (!m_infixParse.TryGetValue(PeekToken.Type, out infix))
                    return leftExpression;

                NextToken();

                leftExpression = infix(leftExpression);
            }

            return leftExpression;
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

        private PrefixExpression ParsePrefixExpression()
        {
            var expression = new PrefixExpression { Token = CurrentToken };
            expression.Operator = CurrentToken.Literal;

            NextToken();

            expression.Right = ParseExpression(Precedence.Prefix);

            return expression;
        }

        private InfixExpression ParseInfixExpression(Expression left)
        {
            var expression = new InfixExpression { Token = CurrentToken };
            expression.Left = left;
            expression.Operator = CurrentToken.Literal;

            var precedence = CurrentPrecedence();

            NextToken();

            expression.Right = ParseExpression(precedence);

            return expression;
        }

        private Boolean ParseBoolean()
        {
            return new Boolean { Token = CurrentToken, Value = CurrentTokenIs(TokenType.TRUE) };
        }

        private Expression ParseGroupedExpression()
        {
            NextToken();

            var expression = ParseExpression(Precedence.Lowest);

            if (!ExpectPeek(TokenType.RPAREN))
                return null;

            return expression;
        }

        private Precedence CurrentPrecedence()
        {
            if (m_precedences.TryGetValue(CurrentToken.Type, out var precedence))
                return precedence;
            else
                return Precedence.Lowest;
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

        private void NoPrefixError(TokenType type)
        {
            m_errors.Add($"No prefix parse function for {type} found.");
        }

        private void PeekError(TokenType tokenType)
        {
            m_errors.Add($"Expected next token to be {tokenType}, got {PeekToken.Type} instead.");
        }

        private Precedence PeekPrecedence()
        {
            if (m_precedences.TryGetValue(PeekToken.Type, out var precedence))
                return precedence;
            else
                return Precedence.Lowest;
        }

        private bool PeekTokenIs(TokenType type)
        {
            return PeekToken.Type == type;
        }

    }
}