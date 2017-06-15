using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fort
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        
    }
   
}
