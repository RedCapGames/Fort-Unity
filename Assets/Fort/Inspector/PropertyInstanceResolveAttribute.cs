using System;
using System.Reflection;

namespace Fort.Inspector
{
    public class PropertyInstanceResolveAttribute : Attribute
    {
        public Type PropertyInstanceResolver { get; private set; }

        public PropertyInstanceResolveAttribute(Type propertyInstanceResolver)
        {
            PropertyInstanceResolver = propertyInstanceResolver;
        }
    }

    public interface IPropertyInstanceResolver
    {
        InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property);        
    }

    public sealed class InstanceResolverResult
    {
        public InstanceResolverResult()
        {
            PresentableInstanceTokens = new InstanceToken[0];
            PossibleInstanceTokens = new InstanceToken[0];
            IsEditable = false;
            UseValueTypeForEdit = false;
        }
        public InstanceToken[] PresentableInstanceTokens { get; set; }
        public InstanceToken[] PossibleInstanceTokens { get; set; }
        public bool IsEditable { get; set; }
        public bool UseValueTypeForEdit { get; set; }
    }
    public sealed class InstanceToken
    {
        public InstanceToken(string displayName,object value)
        {
            DisplayName = displayName;
            Value = value;
            
        }

        public string DisplayName { get; private set; }
        public object Value { get; private set; }
        
    }
}