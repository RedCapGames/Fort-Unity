using System;
using System.Reflection;

namespace Fort.Inspector
{
    public interface IPresentationResolver
    {
        Presentation Resolve(PresentationResolverParameter parameter);
    }

    public class PresentationResolverParameter
    {
        public Type DataType { get; private set; }
        public object Instance { get; private set; }
        public PresentationSite PresentationSite { get; private set; }

        public PresentationResolverParameter(Type dataType,object instance, PresentationSite presentationSite)
        {
            DataType = dataType;
            Instance = instance;
            PresentationSite = presentationSite;
        }
    }
}