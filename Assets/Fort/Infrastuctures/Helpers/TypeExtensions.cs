using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fort
{
    public static class TypeHelper
    {
        #region  Public Methods

        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static Type[] GetAllTypes(AllTypeCategory category)
        {
            switch (category)
            {
                case AllTypeCategory.Game:
                    return
                        AppDomain.CurrentDomain.GetAssemblies()
                            .Where(
                                assembly =>
                                    assembly.GetName().Name == "Assembly-CSharp" ||
                                    assembly.GetName().Name == "Fort-Game-Plugin")
                            .SelectMany(assembly => assembly.GetTypes())
                            .ToArray();
                case AllTypeCategory.Editor:
                    return
                        AppDomain.CurrentDomain.GetAssemblies()
                            .Where(
                                assembly =>
                                    assembly.GetName().Name == "Assembly-CSharp-Editor" ||
                                    assembly.GetName().Name == "Fort-Editor-Plugin")
                            .SelectMany(assembly => assembly.GetTypes())
                            .ToArray();
                case AllTypeCategory.All:
                    return
                        AppDomain.CurrentDomain.GetAssemblies()
                            .Where(
                                assembly =>
                                    assembly.GetName().Name == "Assembly-CSharp" ||
                                    assembly.GetName().Name == "Assembly-CSharp-Editor" ||
                                    assembly.GetName().Name == "Fort-Game-Plugin" ||
                                    assembly.GetName().Name == "Fort-Editor-Plugin")
                            .SelectMany(assembly => assembly.GetTypes())
                            .ToArray();
                default:
                    throw new ArgumentOutOfRangeException("category", category, null);
            }
        }

        public static object[] FindType(object target, Type type)
        {
            return InternalFindType(target, type, new List<object>());
        }

        #endregion

        #region Private Methods

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
                IDictionary dictionary = (IDictionary) target;
                foreach (object value in dictionary.Values)
                {
                    result.AddRange(InternalFindType(value, type, traversedObjects));
                }
            }
            else if (target is IEnumerable)
            {
                IEnumerable enumerable = (IEnumerable) target;
                foreach (object value in enumerable)
                {
                    result.AddRange(InternalFindType(value, type, traversedObjects));
                }
            }
            else
            {
                foreach (
                    PropertyInfo propertyInfo in
                        target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    try
                    {
                        result.AddRange(InternalFindType(propertyInfo.GetValue(target, new object[0]), type,
                            traversedObjects));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return result.ToArray();
        }

        #endregion
    }

    public enum AllTypeCategory
    {
        Game,
        Editor,
        All
    }
}