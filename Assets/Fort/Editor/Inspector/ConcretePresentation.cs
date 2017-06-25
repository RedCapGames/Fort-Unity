using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Fort.Inspector
{
    class ConcretePresentation : Presentation
    {
        
        private Type _objectType;
        private PresentationField[] _presentationFieldInfos;
        private bool _isFoldout;
        PropertyInfo[] GetAllProperties(Type baseType)
        {
            if(baseType.GetCustomAttribute<IgnorePresentationAttribute>() != null)
                return new PropertyInfo[0];
            PropertyInfo[] propertyInfos = baseType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (baseType.BaseType == null)
                return propertyInfos;
            return propertyInfos.Where(info => info.CanRead && info.CanWrite).Concat(GetAllProperties(baseType.BaseType)).ToArray();
        }
        private void Initialize()
        {
            if (_presentationFieldInfos != null)
                return;
            _presentationFieldInfos =
                GetAllProperties(_objectType).Where(info => info.GetCustomAttribute<IgnorePresentationAttribute>()==null && info.CanRead && info.CanWrite)
                    .Select(
                        info =>
                            new PresentationField
                            {
                                PropertyInfo = info
                            })
                    .ToArray();
        }



        #region Overrides of PresentationFieldInfo

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            if (parameter.PresentationSite != null && parameter.PresentationSite.Base != null &&
                parameter.DataType == parameter.PresentationSite.Base.GetType() && parameter.Instance == null)
            {
                return OnCircularRefrence(parameter);
            }
            bool isRoot = string.IsNullOrEmpty(parameter.Title);
            if (parameter.Instance != null && _objectType == null)
                _objectType = parameter.Instance.GetType();
            else
                _objectType = parameter.DataType;
            Initialize();
            //bool isChanged = false;
            Change change = new Change();
            object presentationData = parameter.PresentationData;
            ConcretePresentationData concretePresentationData;
            if (presentationData != null && !((presentationData) is ConcretePresentationData))
                presentationData = null;
            if (presentationData != null)
            {
                concretePresentationData = (ConcretePresentationData)presentationData;                
                _isFoldout = concretePresentationData.IsFoldout;
            }
            else
            {
                presentationData = concretePresentationData = new ConcretePresentationData { IsFoldout = _isFoldout };
            }
            if (!isRoot)
            {
                concretePresentationData.IsFoldout = EditorGUILayout.Foldout(_isFoldout, parameter.Title);
                change.IsPresentationChanged |= _isFoldout != concretePresentationData.IsFoldout;
                _isFoldout = concretePresentationData.IsFoldout;
                EditorGUILayout.BeginHorizontal();
                GUILayoutUtility.GetRect(3f, 6f);
            }
            
            EditorGUILayout.BeginVertical();
            object data = parameter.Instance;
            if (data == null)
            {
                data = Activator.CreateInstance(_objectType);
                change.IsDataChanged = true;
            }
            if (isRoot || _isFoldout)
            {
                change.ChildrenChange = new Change[_presentationFieldInfos.Length];
                int i = 0;
                foreach (PresentationField presentationField in _presentationFieldInfos)
                {
                    if (!concretePresentationData.InnerPresentationData.ContainsKey(presentationField.PropertyInfo.Name))
                        concretePresentationData.InnerPresentationData[presentationField.PropertyInfo.Name] = null;
                    object objectData = presentationField.PropertyInfo.GetValue(data, new object[0]);
                    PresentationSite presentationSite = new PresentationSite
                    {
                        BasePresentation = this,
                        Base = parameter.Instance,
                        BaseSite = parameter.PresentationSite,
                        PropertyInfo = presentationField.PropertyInfo,
                        SiteType = PresentationSiteType.Property
                    };
                    if (presentationField.Presentation == null)
                    {
                        PresentationResolverParameter resolverParameter =
                            new PresentationResolverParameter(presentationField.PropertyInfo.PropertyType, objectData,
                                presentationSite);
                        presentationField.Presentation =
                            parameter.FortInspector.GetResolver().Resolve(resolverParameter);
                    }
                    //PresentationParamater 
                    PresentationTitleAttribute presentationTitleAttribute = presentationField.PropertyInfo.GetCustomAttribute<PresentationTitleAttribute>();
                    string title = presentationTitleAttribute == null
                        ? CamelCaseSplit.SplitCamelCase(presentationField.PropertyInfo.Name) 
                        : presentationTitleAttribute.Title;
                    PresentationResult presentationResult =
                        presentationField.Presentation.OnInspectorGui(
                            new PresentationParamater(presentationField.PropertyInfo.GetValue(data, new object[0]),
                                concretePresentationData.InnerPresentationData[presentationField.PropertyInfo.Name],
                                title, presentationField.PropertyInfo.PropertyType,
                                presentationSite, parameter.FortInspector));
                    presentationField.PropertyInfo.SetValue(data, presentationResult.Result, new object[0]);
                    change.ChildrenChange[i] = presentationResult.Change;
                    concretePresentationData.InnerPresentationData[presentationField.PropertyInfo.Name] =
                        presentationResult.PresentationData;
                    i++;
                }
            }
            EditorGUILayout.EndVertical();
            if (!isRoot)
            {
                EditorGUILayout.EndHorizontal();
            }
            return new PresentationResult
            {
                Result = data,
                Change = change,
                PresentationData = presentationData
            };
        }

        #endregion

        private PresentationResult OnCircularRefrence(PresentationParamater paramater)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(paramater.Title);
            bool isDataChanged = false;
            object result = null;
            if (GUILayout.Button("Add"))
            {
                result = Activator.CreateInstance(paramater.DataType);
                isDataChanged = true;
            }
            EditorGUILayout.EndHorizontal();
            return new PresentationResult {Change = new Change {IsDataChanged = isDataChanged },Result = result };
        }
        private class PresentationField
        {
            public Presentation Presentation { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
            
        }

        [Serializable]
        class ConcretePresentationData
        {
            public ConcretePresentationData()
            {
                InnerPresentationData = new Dictionary<string, object>();
            }
            public bool IsFoldout { get; set; }
            public Dictionary<string,object> InnerPresentationData { get; set; } 
        }
    }
}