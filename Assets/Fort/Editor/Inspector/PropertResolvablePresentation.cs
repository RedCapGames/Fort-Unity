using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Utilities;
using UnityEditor;
using UnityEngine;

namespace Fort.Inspector
{
    public class PropertyResolvablePresentation : Presentation
    {
        private IPropertyInstanceResolver _propertyInstanceResolver;
        #region Overrides of Presentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            if (_propertyInstanceResolver == null)
                _propertyInstanceResolver =
                    (IPropertyInstanceResolver)
                        Activator.CreateInstance(
                            parameter.PresentationSite.PropertyInfo.GetCustomAttribute<PropertyInstanceResolveAttribute>
                                ().PropertyInstanceResolver);
            Change change = new Change();
            object presentationData = parameter.PresentationData;
            if (presentationData != null && !((presentationData) is PropertyResolvablePresentationData))
                presentationData = null;
            PropertyResolvablePresentationData propertyResolvablePresentationData;
            if (presentationData != null)
            {
                propertyResolvablePresentationData = (PropertyResolvablePresentationData)presentationData;
            }
            else
            {
                propertyResolvablePresentationData = new PropertyResolvablePresentationData();
            }
            bool oldFoldout = propertyResolvablePresentationData.IsFoldout;
            propertyResolvablePresentationData.IsFoldout = EditorGUILayout.Foldout(propertyResolvablePresentationData.IsFoldout, parameter.Title);
            change.IsPresentationChanged |= propertyResolvablePresentationData.IsFoldout != oldFoldout;
            object result = parameter.Instance;
            if (propertyResolvablePresentationData.IsFoldout)
            {
                InstanceResolverResult instanceResolverResult = _propertyInstanceResolver.ResolvePossibleData(parameter.PresentationSite.Base, parameter.Instance,
                    parameter.PresentationSite.PropertyInfo);
                if (parameter.DataType.IsArray)
                {
                    EditorGUILayout.BeginVertical();
                    List<int> removedIndex = new List<int>();
                    object[] innerPresentationData = propertyResolvablePresentationData.PresentationDatas;
                    if(innerPresentationData == null || innerPresentationData.Length == 0)
                        innerPresentationData = new object[instanceResolverResult.PresentableInstanceTokens.Length];
                    object[] finalInnerPresentationData = new object[instanceResolverResult.PresentableInstanceTokens.Length];
                    for (int i = 0; i < instanceResolverResult.PresentableInstanceTokens.Length; i++)
                    {
                        finalInnerPresentationData[i] = i < innerPresentationData.Length
                            ? innerPresentationData[i]
                            : null;
                    }
                    object[] results = new object[instanceResolverResult.PresentableInstanceTokens.Length];
                    change.ChildrenChange = new Change[instanceResolverResult.PresentableInstanceTokens.Length];
                    for (int i = 0; i < instanceResolverResult.PresentableInstanceTokens.Length; i++)
                    {
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(instanceResolverResult.PresentableInstanceTokens[i].DisplayName);
                        if (GUILayout.Button("-"))
                        {
                            removedIndex.Add(i);
                            change.IsDataChanged = true;
                        }
                        EditorGUILayout.EndHorizontal();
                        if (instanceResolverResult.IsEditable)
                        {
                            PresentationSite presentationSite = new PresentationSite
                            {
                                BaseSite = parameter.PresentationSite,
                                BasePresentation = this,
                                Base = parameter.Instance,
                                SiteType = PresentationSiteType.None
                            };
                            Type elementType = instanceResolverResult.UseValueTypeForEdit
                                ? instanceResolverResult.PresentableInstanceTokens[i].Value.GetType()
                                : parameter.DataType.GetElementType();
                            Presentation presentation =
                            parameter.FortInspector.GetResolver()
                                .Resolve(new PresentationResolverParameter(elementType, instanceResolverResult.PresentableInstanceTokens[i].Value,
                                    presentationSite));
                            PresentationResult presentationResult =
                                presentation.OnInspectorGui(
                                    new PresentationParamater(
                                        instanceResolverResult.PresentableInstanceTokens[i].Value,
                                        finalInnerPresentationData[i], string.Format("Element {0}", i),
                                        elementType, presentationSite, parameter.FortInspector));
                            results[i] = presentationResult.Result;
                            finalInnerPresentationData[i] = presentationResult.PresentationData;
                            change.ChildrenChange[i] = presentationResult.Change;                            
                        }
                        EditorGUILayout.EndVertical();
                    }
                    propertyResolvablePresentationData.PresentationDatas = finalInnerPresentationData;
                    object addObject = null;
                    if (instanceResolverResult.PossibleInstanceTokens.Length > 0)
                    {
                        int selectedIndex = EditorGUILayout.Popup("New Element", 0,
                            new[] { "None" }.Concat(
                                instanceResolverResult.PossibleInstanceTokens.Select(token => token.DisplayName))
                                .ToArray());
                        if (selectedIndex != 0)
                        {
                            addObject = instanceResolverResult.PossibleInstanceTokens[selectedIndex - 1].Value;
                            change.IsDataChanged = true;
                        }
                    }
                    int firstArraySize = parameter.Instance is Array ? ((Array)parameter.Instance).Length : 0;
                    int arraySize = firstArraySize - removedIndex.Count + (addObject != null ? 1 : 0);
                    result = parameter.Instance;
                    if (result == null || arraySize != firstArraySize)
                    {
                        result = Array.CreateInstance(parameter.DataType.GetElementType(), arraySize);
                    }
                    Array array = (Array)result;
                    int resultIndex = 0;
                    for (int i = 0; i < instanceResolverResult.PresentableInstanceTokens.Length; i++)
                    {
                        if (removedIndex.Contains(i))
                            continue;
                        array.SetValue(instanceResolverResult.PresentableInstanceTokens[i].Value, resultIndex);
                        resultIndex++;
                    }
                    if (addObject != null)
                        array.SetValue(addObject, array.Length - 1);
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    int selectedIndex = parameter.Instance == null || instanceResolverResult.PresentableInstanceTokens.Length==0
                        ? 0
                        : instanceResolverResult.PossibleInstanceTokens.ToList().IndexOf(instanceResolverResult.PresentableInstanceTokens.Single()) + 1;

                    selectedIndex = EditorGUILayout.Popup("", selectedIndex,
                            new[] { "None" }.Concat(
                                instanceResolverResult.PossibleInstanceTokens.Select(token => token.DisplayName))
                                .ToArray());

                    if (selectedIndex == 0)
                    {
                        result = null;
                    }
                    else
                    {
                        result = instanceResolverResult.PossibleInstanceTokens[selectedIndex - 1].Value;
                    }
                    if (result != parameter.Instance)
                    {
                        change.IsDataChanged = true;
                    }
                }
            }

            return new PresentationResult
            {
                Result = result,
                Change = change,
                PresentationData = propertyResolvablePresentationData
            };
        }

        #endregion        
        class PropertyResolvablePresentationData
        {
            public bool IsFoldout { get; set; }
            public object[] PresentationDatas { get; set; }

        }
    }
}