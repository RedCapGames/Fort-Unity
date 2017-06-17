using System;
using System.Collections.Generic;
using System.Linq;
using Fort.Info;

namespace Fort.Inspector
{
    class PresentationResolver: IPresentationResolver
    {
        Dictionary<Type,Type> _numericTypes = new Dictionary<Type, Type>
        {
            {typeof(byte), typeof(byte)},
            {typeof(sbyte), typeof(sbyte)},
            {typeof(short), typeof(short)},
            {typeof(ushort), typeof(ushort)},
            {typeof(int), typeof(int)},
            {typeof(uint), typeof(uint)},
            {typeof(long), typeof(long)},
            {typeof(ulong), typeof(ulong)},
            {typeof(float), typeof(float)},
            {typeof(double), typeof(double)},
            {typeof(decimal), typeof(decimal)}
        };
        #region Implementation of IPresentationFieldInfoResolver

        public Presentation Resolve(PresentationResolverParameter parameter)
        {
            Type type = parameter.DataType;// parameter.Instance == null ? parameter.DataType : parameter.Instance.GetType();
            Presentation presentation;
            if (parameter.PresentationSite.SiteType == PresentationSiteType.Property && parameter.PresentationSite.PropertyInfo.GetCustomAttribute<InspectorAttribute>() != null)
            {
                Type presentationType =
                    GetType()
                        .Assembly.GetTypes()
                        .First(
                            type1 =>
                                string.Format("{0}.{1}", type1.Namespace, type1.Name) ==
                                parameter.PresentationSite.PropertyInfo.GetCustomAttribute<InspectorAttribute>()
                                    .Presentation);
                presentation = (Presentation) Activator.CreateInstance(presentationType);
            }
            else if (parameter.PresentationSite.SiteType == PresentationSiteType.Property &&
                     parameter.PresentationSite.PropertyInfo.GetCustomAttribute<PropertyInstanceResolveAttribute>() != null)
            {
                return new PropertyResolvablePresentation();
            }
            else if (type == typeof(string))
            {
                presentation = new StringPresentation();
            }
            else if (_numericTypes.ContainsKey(type))
            {
                presentation = new NumberPresentation();
            }
            else if (type == typeof(bool))
            {
                presentation = new BoolPresentation();
            }
            else if (type.IsEnum)
            {
                presentation = new EnumPresentation();
            }
            else if (type.IsArray)
            {
                presentation = new ArrayPresentation();
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                presentation = new UnityObjectPresentation();
            }
            else
            {
                InspectorAttribute inspectorAttribute = type.GetCustomAttribute<InspectorAttribute>();
                if (inspectorAttribute == null)
                {
                    if(type.IsAbstract || type.IsInterface)
                        presentation = new AbstractConcretePresentation();
                    else
                        presentation = new ConcretePresentation();
                }
                else
                {
                    Type presentationType = GetType().Assembly.GetTypes().First(type1 => string.Format("{0}.{1}", type1.Namespace, type1.Name) == type.GetCustomAttribute<InspectorAttribute>().Presentation);
                    presentation = (Presentation)Activator.CreateInstance(presentationType);
                }
            }
            return presentation;
        }

        #endregion
    }
}