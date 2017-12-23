using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class Integer : Object, IEquatable<Integer>
    {
        public Integer()
        {

        }

        public override ObjectType Type => ObjectType.Integer;

        public Int64 Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }

        #region IEquatable<Integer> Members

        public bool Equals(Integer other)
        {
            return Value == other.Value;
        }

        #endregion
    }
}
