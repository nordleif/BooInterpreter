using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class Boolean : Object, IEquatable<Boolean>
    {
        public Boolean()
        {

        }

        public override ObjectType Type => ObjectType.Boolean;

        public bool Value { get; set; }

        public override string ToString()
        {
            return $"{Value}".ToLower();
        }

        #region IEquatable<Boolean> Members

        public bool Equals(Boolean other)
        {
            return Value == other.Value;
        }

        #endregion
    }
}
