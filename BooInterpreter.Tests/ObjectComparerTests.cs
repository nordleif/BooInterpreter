using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using BooInterpreter.Objects;
using Array = BooInterpreter.Objects.Array;
using Boolean = BooInterpreter.Objects.Boolean;
using Object = BooInterpreter.Objects.Object;
using String = BooInterpreter.Objects.String;

namespace BooInterpreter
{
    [TestFixture]
    public class ObjectComparerTests
    {
        [Test]
        public void ObjectComparer_StringEquals()
        {
            var hello1 = new String { Value = "Hello World" };
            var hello2 = new String { Value = "Hello World" };
            var diff1 = new String { Value = "My name is johnny" };
            var diff2 = new String { Value = "My name is johnny" };

            Assert.IsTrue(ObjectComparer.Default.Equals(hello1, hello2));
            Assert.IsTrue(ObjectComparer.Default.Equals(diff1, diff2));
            Assert.IsFalse(ObjectComparer.Default.Equals(diff1, hello1));
        }

        [Test]
        public void ObjectComparer_IntegerEquals()
        {
            var hello1 = new Integer { Value = 1 };
            var hello2 = new Integer { Value = 1 };
            var diff1 = new Integer { Value = 2 };
            var diff2 = new Integer { Value = 2 };

            Assert.IsTrue(ObjectComparer.Default.Equals(hello1, hello2));
            Assert.IsTrue(ObjectComparer.Default.Equals(diff1, diff2));
            Assert.IsFalse(ObjectComparer.Default.Equals(diff1, hello1));
        }

        [Test]
        public void ObjectComparer_BooleanEquals()
        {
            var hello1 = new Boolean { Value = true };
            var hello2 = new Boolean { Value = true };
            var diff1 = new Boolean { Value = false };
            var diff2 = new Boolean { Value = false };

            Assert.IsTrue(ObjectComparer.Default.Equals(hello1, hello2));
            Assert.IsTrue(ObjectComparer.Default.Equals(diff1, diff2));
            Assert.IsFalse(ObjectComparer.Default.Equals(diff1, hello1));
        }
    }
}
