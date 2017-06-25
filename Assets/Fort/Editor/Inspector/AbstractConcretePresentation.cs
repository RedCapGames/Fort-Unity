using System;
using System.Linq;
using UnityEditor;

namespace Fort.Inspector
{
    class AbstractConcretePresentation:Presentation
    {
        private Presentation _presentation;
        #region Overrides of Presentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            AbstractConcretePresentationData presentationData =  parameter.PresentationData as AbstractConcretePresentationData ?? new AbstractConcretePresentationData();
            presentationData.IsFoldout = EditorGUILayout.Foldout(presentationData.IsFoldout, parameter.Title);

            object data = parameter.Instance;
            Change change = new Change();
            if (presentationData.IsFoldout)
            {
                Type[] possibleTypes;
                AbstractTypeChildResolverAttribute abstractTypeChildResolverAttribute = parameter.DataType.GetCustomAttribute<AbstractTypeChildResolverAttribute>();
                if (abstractTypeChildResolverAttribute != null)
                {
                    IAbstractTypeChildResolver abstractTypeChildResolver =
                        (IAbstractTypeChildResolver)
                            Activator.CreateInstance(abstractTypeChildResolverAttribute.ChildResolverType);
                    possibleTypes = abstractTypeChildResolver.ResolveChildrenType(parameter.DataType,
                        parameter.PresentationSite.PropertyInfo);
                }
                else
                {
                    possibleTypes =
                    TypeHelper.GetAllTypes(AllTypeCategory.Game)
                        .Where(type => parameter.DataType.IsAssignableFrom(type) && !type.IsAbstract)
                        .ToArray();
                }


                int selectedIndex = 0;
                if (parameter.Instance != null)
                {
                    selectedIndex = possibleTypes.ToList().IndexOf(parameter.Instance.GetType()) + 1;

                }
                selectedIndex = EditorGUILayout.Popup("Class Type", selectedIndex,
                    new[] { "None" }.Concat(possibleTypes.Select(type =>
                    {
                        PresentationTitleAttribute presentationTitleAttribute = type.GetCustomAttribute<PresentationTitleAttribute>();
                        return presentationTitleAttribute == null ? CamelCaseSplit.SplitCamelCase(type.Name) : presentationTitleAttribute.Title;
                    })).ToArray());
                object oldData = data;
                bool changed = false;
                if (selectedIndex > 0)
                {
                    if (data == null || data.GetType() != possibleTypes[selectedIndex - 1])
                    {
                        data = Activator.CreateInstance(possibleTypes[selectedIndex - 1]);
                        changed = true;
                    }
                }
                else
                {
                    data = null;
                }
                if (data != oldData)
                {
                    change.IsDataChanged = true;
                }
                //object presentationData = parameter.PresentationData;
                if (data != null)
                {
                    PresentationSite presentationSite = new PresentationSite
                    {
                        BaseSite = parameter.PresentationSite,
                        BasePresentation = this,
                        Base = parameter.Instance,
                        SiteType = PresentationSiteType.None
                    };
                    if (_presentation == null || changed)
                    {
                        _presentation =
                            parameter.FortInspector.GetResolver()
                                .Resolve(new PresentationResolverParameter(possibleTypes[selectedIndex - 1], data,
                                    presentationSite));
                    }
                    PresentationResult presentationResult = _presentation.OnInspectorGui(new PresentationParamater(data, presentationData.InnerPresentationData,
                        string.Empty, possibleTypes[selectedIndex - 1], presentationSite, parameter.FortInspector));
                    presentationData.InnerPresentationData = presentationResult.PresentationData;
                    data = presentationResult.Result;
                    change.ChildrenChange = new[] { presentationResult.Change };
                }
            }
            
            return new PresentationResult
            {
                Change = change,
                PresentationData = presentationData,
                Result = data
            };
        }

        #endregion

        class AbstractConcretePresentationData
        {
            public bool IsFoldout { get; set; }
            public object InnerPresentationData { get; set; }
        }
    }
}
