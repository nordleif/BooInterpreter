using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BooInterpreter
{
    [TestFixture]
    public class AstTests
    {
        [Test]
        public void Ast_TestString()
        {
            var program = new Program
            {
                Statements = new Statement[] {
                    new LetStatement {
                        Token = new Token(TokenType.LET, "let"),
                        Name = new Identifier { Token = new Token(TokenType.IDENT, "myVar"), Value = "myVar" },
                        Value = new Identifier { Token = new Token(TokenType.IDENT, "anotherVar"), Value = "anotherVar" }
                    }
                }                 
            };

            Assert.AreEqual("let myVar = anotherVar;", program.ToString());
        }
    }
}
