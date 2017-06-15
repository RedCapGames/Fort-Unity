using System;
using System.Reflection;

namespace Fort.Inspector
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AbstractTypeChildResolverAttribute : Attribute
    {
        public Type ChildResolverType { get; private set; }

        public AbstractTypeChildResolverAttribute(Type childResolverType)
        {
            ChildResolverType = childResolverType;
        }
    }

    public interface IAbstractTypeChildResolver
    {
        Type[] ResolveChildrenType(Type baseType, PropertyInfo propertyInfo);
    }
}