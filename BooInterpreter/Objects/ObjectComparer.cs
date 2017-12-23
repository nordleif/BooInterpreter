using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class ObjectComparer : IEqualityComparer<Object>
    {
        #region Static Members

        private static ObjectComparer m_default;

        public static ObjectComparer Default
        {
            get
            {
                if (m_default == null)
                    m_default = new ObjectComparer();
                return m_default;
            }
        }

        #endregion

        public ObjectComparer()
        {

        }

        #region IEqualityComparer<Object> Members

        public bool Equals(Object x, Object y)
        {
            if (x is String xString && y is String yString)
                return xString.Value.Equals(yString.Value);
            if (x is Integer xInteger && y is Integer yInteger)
                return xInteger.Value.Equals(yInteger.Value);
            if (x is Boolean xBoolean && y is Boolean yBoolean)
                return xBoolean.Value.Equals(yBoolean.Value);
            else
                return x.Equals(y);
        }

        public int GetHashCode(Object obj)
        {
            if (obj is String objString)
                return objString.Value.GetHashCode();
            else if (obj is Integer objInteger)
                return objInteger.Value.GetHashCode();
            else if (obj is Boolean objBoolean)
                return objBoolean.Value.GetHashCode();
            else
                return obj.GetHashCode();
        }

        #endregion
    }
}
