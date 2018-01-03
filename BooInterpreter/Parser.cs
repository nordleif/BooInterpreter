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
            m_prefixParse.Add(TokenType.IF, ParseIfExpression);
            m_prefixParse.Add(TokenType.FUNCTION, ParseFunctionLiteral);
            m_prefixParse.Add(TokenType.STRING, ParseStringLiteral);
            m_prefixParse.Add(TokenType.LBRACKET, ParseArrayLiteral);
            m_prefixParse.Add(TokenType.LBRACE, ParseHashLiteral);
            m_prefixParse.Add(TokenType.WHILE, ParseWhileExpression);

            m_infixParse = new Dictionary<TokenType, Func<Expression, Expression>>();
            m_infixParse.Add(TokenType.PLUS, ParseInfixExpression);
            m_infixParse.Add(TokenType.MINUS, ParseInfixExpression);
            m_infixParse.Add(TokenType.SLASH, ParseInfixExpression);
            m_infixParse.Add(TokenType.ASTERISK, ParseInfixExpression);
            m_infixParse.Add(TokenType.EQ, ParseInfixExpression);
            m_infixParse.Add(TokenType.NOT_EQ, ParseInfixExpression);
            m_infixParse.Add(TokenType.LT, ParseInfixExpression);
            m_infixParse.Add(TokenType.GT, ParseInfixExpression);
            m_infixParse.Add(TokenType.LPAREN, ParseCallExpression);
            m_infixParse.Add(TokenType.LBRACKET, ParseIndexExpression);
            m_infixParse.Add(TokenType.ASSIGN, ParseAssignExpression);

            m_precedences = new Dictionary<TokenType, Precedence>();
            m_precedences.Add(TokenType.ASSIGN, Precedence.Assign);
            m_precedences.Add(TokenType.EQ, Precedence.Equals);
            m_precedences.Add(TokenType.NOT_EQ, Precedence.Equals);
            m_precedences.Add(TokenType.LT, Precedence.LessGreater);
            m_precedences.Add(TokenType.GT, Precedence.LessGreater);
            m_precedences.Add(TokenType.PLUS, Precedence.Sum);
            m_precedences.Add(TokenType.MINUS, Precedence.Sum);
            m_precedences.Add(TokenType.SLASH, Precedence.Product);
            m_precedences.Add(TokenType.ASTERISK, Precedence.Product);
            m_precedences.Add(TokenType.LPAREN, Precedence.Call);
            m_precedences.Add(TokenType.LBRACKET, Precedence.Index);

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

        private AssignExpression ParseAssignExpression(Expression left)
        {
            var expression = new AssignExpression { Token = CurrentToken };

            var identifier = left as Identifier;
            if (identifier == null)
                return null;

            expression.Name = identifier;
            
            var precedence = CurrentPrecedence();

            NextToken();

            expression.Value = ParseExpression(precedence);

            return expression;
        }

        private BooleanLiteral ParseBoolean()
        {
            return new BooleanLiteral { Token = CurrentToken, Value = CurrentTokenIs(TokenType.TRUE) };
        }

        private Expression ParseGroupedExpression()
        {
            NextToken();

            var expression = ParseExpression(Precedence.Lowest);

            if (!ExpectPeek(TokenType.RPAREN))
                return null;

            return expression;
        }

        private Expression ParseIfExpression()
        {
            var expression = new IfExpression { Token = CurrentToken };

            if (!ExpectPeek(TokenType.LPAREN))
                return null;

            NextToken();

            expression.Condition = ParseExpression(Precedence.Lowest);

            if (!ExpectPeek(TokenType.RPAREN))
                return null;

            if (!ExpectPeek(TokenType.LBRACE))
                return null;

            expression.Consequence = ParseBlockStatement();

            if (PeekTokenIs(TokenType.ELSE))
            {
                NextToken();

                if (!ExpectPeek(TokenType.LBRACE))
                    return null;

                expression.Alternative = ParseBlockStatement();
            }

            return expression;
        }

        private WhileExpression ParseWhileExpression()
        {
            var expression = new WhileExpression { Token = CurrentToken };

            if (!ExpectPeek(TokenType.LPAREN))
                return null;

            NextToken();

            expression.Condition = ParseExpression(Precedence.Lowest);

            if (!ExpectPeek(TokenType.RPAREN))
                return null;

            if (!ExpectPeek(TokenType.LBRACE))
                return null;

            expression.Statement = ParseBlockStatement();
            
            return expression;
        }

        private BlockStatement ParseBlockStatement()
        {
            var block = new BlockStatement { Token = CurrentToken };
            
            NextToken();

            var statements = new List<Statement>();
            while(!CurrentTokenIs(TokenType.RBRACE) && !CurrentTokenIs(TokenType.EOF))
            {
                var statement = ParseStatement();
                if (statement != null)
                    statements.Add(statement);
                NextToken();
            }
            block.Statements = statements.ToArray();
            

            return block;
        }

        private FunctionLiteral ParseFunctionLiteral()
        {
            var literal = new FunctionLiteral { Token = CurrentToken };

            if (!ExpectPeek(TokenType.LPAREN))
                return null;

            literal.Parameters = ParseFunctionParameters();

            if (!ExpectPeek(TokenType.LBRACE))
                return null;

            literal.Body = ParseBlockStatement();
            
            return literal;
        }

        private Identifier[] ParseFunctionParameters()
        {
            var identifiers = new List<Identifier>();

            if (PeekTokenIs(TokenType.RPAREN))
            {
                NextToken();
                return identifiers.ToArray();
            }

            NextToken();
            
            identifiers.Add(new Identifier { Token = CurrentToken, Value = CurrentToken.Literal });

            while(PeekTokenIs(TokenType.COMMA))
            {
                NextToken();
                NextToken();
                identifiers.Add(new Identifier { Token = CurrentToken, Value = CurrentToken.Literal });
            }

            if (!ExpectPeek(TokenType.RPAREN))
                return null;

            return identifiers.ToArray();
        }

        private CallExpression ParseCallExpression(Expression function)
        {
            var expression = new CallExpression { Token = CurrentToken };
            expression.Function = function;
            expression.Arguments = ParseExpressionList(TokenType.RPAREN);

            return expression;
        }

        private StringLiteral ParseStringLiteral()
        {
            var literal = new StringLiteral { Token = CurrentToken };
            literal.Value = CurrentToken.Literal;
            return literal;
        }

        private ArrayLiteral ParseArrayLiteral()
        {
            var literal = new ArrayLiteral { Token = CurrentToken };
            literal.Elements = ParseExpressionList(TokenType.RBRACKET);
            return literal;
        }

        private Expression[] ParseExpressionList(TokenType end)
        {
            var list = new List<Expression>();

            if (PeekTokenIs(end))
            {
                NextToken();
                return list.ToArray();
            }

            NextToken();

            list.Add(ParseExpression(Precedence.Lowest));

            while(PeekTokenIs(TokenType.COMMA))
            {
                NextToken();
                NextToken();
                list.Add(ParseExpression(Precedence.Lowest));
            }

            if (!ExpectPeek(end))
                return null;
            
            return list.ToArray();
        }

        private IndexExpression ParseIndexExpression(Expression left)
        {
            var expression = new IndexExpression { Token = CurrentToken };
            expression.Left = left;

            NextToken();

            expression.Index = ParseExpression(Precedence.Lowest);

            if (!ExpectPeek(TokenType.RBRACKET))
                return null;

            return expression;
        }

        private HashLiteral ParseHashLiteral()
        {
            var literal = new HashLiteral { Token = CurrentToken };
            literal.Pairs = new Dictionary<Expression, Expression>();

            while(!PeekTokenIs(TokenType.RBRACE))
            {
                NextToken();

                var key = ParseExpression(Precedence.Lowest);

                if (!ExpectPeek(TokenType.COLON))
                    return null;

                NextToken();

                var value = ParseExpression(Precedence.Lowest);

                literal.Pairs[key] = value;

                if (!PeekTokenIs(TokenType.RBRACE) && !ExpectPeek(TokenType.COMMA))
                    return null;
            }
            
            if (!ExpectPeek(TokenType.RBRACE))
                return null;
            
            return literal;
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