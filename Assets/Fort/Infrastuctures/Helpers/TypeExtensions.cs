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

        public static Type[] GetAllTypes()
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(
                        assembly =>
                            assembly.GetName().Name == "Assembly-CSharp" ||
                            assembly.GetName().Name == "Assembly-CSharp-Editor"||
                            assembly.GetName().Name == "Fort-Game-Plugin" ||
                            assembly.GetName().Name == "Fort-Editor-Plugin")
                    .SelectMany(assembly => assembly.GetTypes())
                    .ToArray();
        }
        
    }
   
}
