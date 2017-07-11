﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Fort.Inspector
{
    class AbstractConcretePresentation:Presentation
    {
        private Presentation _presentation;

        private PropertyInfo[] ResolvePresentationSiteProperties(PresentationSite presentationSite)
        {
            List<PropertyInfo> result = new List<PropertyInfo>();
            while (presentationSite != null)
            {
                if(presentationSite.PropertyInfo != null)
                    result.Add(presentationSite.PropertyInfo);
                presentationSite = presentationSite.BaseSite;
            }
            return result.ToArray();
        }
        #region Overrides of Presentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            GUIStyle guiStyle = new GUIStyle();
            AbstractConcretePresentationData presentationData =  parameter.PresentationData as AbstractConcretePresentationData ?? new AbstractConcretePresentationData();
            Change change = new Change();
            bool isFoldout = presentationData.IsFoldout;
            presentationData.IsFoldout = EditorGUILayout.Foldout(presentationData.IsFoldout, parameter.Title);
            change.IsPresentationChanged = isFoldout != presentationData.IsFoldout;
            object data = parameter.Instance;
            
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
                        ResolvePresentationSiteProperties(parameter.PresentationSite));

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
                EditorGUILayout.BeginHorizontal(guiStyle);
                GUILayout.Space(FortInspector.ItemSpacing);
                EditorGUILayout.BeginVertical(guiStyle);
                selectedIndex = EditorGUILayout.Popup("Class Type", selectedIndex,
                    new[] { "None" }.Concat(possibleTypes.Select(type =>
                    {
                        PresentationTitleAttribute presentationTitleAttribute = type.GetCustomAttribute<PresentationTitleAttribute>();
                        return presentationTitleAttribute == null ? CamelCaseSplit.SplitCamelCase(type.Name) : presentationTitleAttribute.Title;
                    })).ToArray());
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
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
                    EditorGUILayout.BeginHorizontal(guiStyle);
                    GUILayout.Space(FortInspector.ItemSpacing);
                    EditorGUILayout.BeginVertical(guiStyle);
                    PresentationResult presentationResult = _presentation.OnInspectorGui(new PresentationParamater(data, presentationData.InnerPresentationData,
                        string.Empty, possibleTypes[selectedIndex - 1], presentationSite, parameter.FortInspector));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
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
