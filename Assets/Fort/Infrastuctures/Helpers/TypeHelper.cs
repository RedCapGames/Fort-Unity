using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Fort
{
    public static class TypeHelper
    {
        #region  Public Methods

        private static void InternalGetAddAllParent(Type type, List<Type> types)
        {
            types.Add(type);
            if(type.BaseType != null)
                InternalGetAddAllParent(type.BaseType,types);
        }
        public static Type[] GetAllParent(Type type)
        {
            List<Type> types = new List<Type>();
            InternalGetAddAllParent(type,types);
            return types.ToArray();
        }
        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private static bool IsTypeMachPlugin(string assemblyName, AllTypeCategory category)
        {
            
            if (!assemblyName.StartsWith("Fort"))
                return false;
            Regex regex = new Regex(@"^Fort([a-z,A-Z]*)\-(Editor|Game)\-Plugin$", RegexOptions.IgnoreCase);
            Match match = regex.Match(assemblyName);
            if (!match.Success)
                return false;
            switch (category)
            {
                case AllTypeCategory.Game:
                    return match.Groups[1].Value == "Game";
                case AllTypeCategory.Editor:
                    return match.Groups[1].Value == "Editor";
                case AllTypeCategory.All:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException("category", category, null);
            }
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
                                    IsTypeMachPlugin(assembly.GetName().Name,AllTypeCategory.Game))
                            .SelectMany(assembly => assembly.GetTypes())
                            .ToArray();
                case AllTypeCategory.Editor:
                    return
                        AppDomain.CurrentDomain.GetAssemblies()
                            .Where(
                                assembly =>
                                    assembly.GetName().Name == "Assembly-CSharp-Editor" ||
                                    IsTypeMachPlugin(assembly.GetName().Name, AllTypeCategory.Editor))
                            .SelectMany(assembly => assembly.GetTypes())
                            .ToArray();
                case AllTypeCategory.All:
                    return
                        AppDomain.CurrentDomain.GetAssemblies()
                            .Where(
                                assembly =>
                                    assembly.GetName().Name == "Assembly-CSharp" ||
                                    assembly.GetName().Name == "Assembly-CSharp-Editor" ||
                                    IsTypeMachPlugin(assembly.GetName().Name, AllTypeCategory.All))
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
            if (type.ContainsGenericParameters && GetAllParent(target.GetType()).Any(type1 => type1.IsGenericTypeDefinition && type1.GetGenericTypeDefinition() == type))
            {
                result.Add(target);
            }
            else if (type.IsInstanceOfType(target))
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
                        target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(info => info.CanRead && info.CanWrite))
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

        public static T GetCustomAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            return propertyInfo.GetCustomAttributes(typeof (T), true).FirstOrDefault() as T;
        }

        public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        }

        public static Type EditorType(string fullName)
        {
            return
                GetAllTypes(AllTypeCategory.Editor)
                    .First(type => string.Format("{0}.{1}", type.Namespace, type.Name) == fullName);
        }
    }

    public enum AllTypeCategory
    {
        Game,
        Editor,
        All
    }
}