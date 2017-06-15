using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fort
{
    public static class TypeFinder
    {
        private static object[] InternalFindType(object target, Type type, List<object> traversedObjects)
        {
            if (target == null)
                return new object[0];

            if (traversedObjects.Contains(target))
                return new object[0];
            traversedObjects.Add(target);
            List<object> result = new List<object>();
            if (target.GetType() == type)
            {
                result.Add(target);
            }
            else if (target is string)
            {
            }
            else if (target.GetType().IsPrimitive)
            {
            }
            else if (target is IDictionary)
            {
                IDictionary dictionary = (IDictionary)target;
                foreach (object value in dictionary.Values)
                {
                    result.AddRange(InternalFindType(value,type,traversedObjects));
                }
            }
            else if (target is IEnumerable)
            {
                IEnumerable enumerable = (IEnumerable)target;
                foreach (object value in enumerable)
                {
                    result.AddRange(InternalFindType(value, type, traversedObjects));
                }
            }
            else
            {
                foreach (PropertyInfo propertyInfo in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    try
                    {
                        result.AddRange(InternalFindType(propertyInfo.GetValue(target, new object[0]), type, traversedObjects));
                    }
                    catch (Exception)
                    {
                    } 
                }
            }
            return result.ToArray();
        }

        public static object[] FindType(object target, Type type)
        {
            return InternalFindType(target, type, new List<object>());
        }
    }
}
