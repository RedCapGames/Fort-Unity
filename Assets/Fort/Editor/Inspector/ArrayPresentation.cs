using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fort.Inspector
{
    public class ArrayPresentation : Presentation
    {
        //private string _titleName;
        //private Type _arrayType;
        private bool _initizalied;
        private bool _isFoldout;        
        private int _elementSize;        
        private readonly string _sizeControlName = Guid.NewGuid().ToString();
        Presentation[] _arrayElementPresentations = new Presentation[0];

        #region Overrides of Presentation

        private void Initialize(Array arrayData, PresentationParamater parameter)
        {
            if (_initizalied)
                return;
            _initizalied = true;
            _elementSize = arrayData == null ? 0 : arrayData.Length;
            Array arrayInstance = (Array)parameter.Instance;            
            _arrayElementPresentations = new Presentation[0];
            if (arrayInstance != null)
            {
                _elementSize = arrayInstance.Length;
                _arrayElementPresentations = arrayInstance.Cast<object>()
                    .Select(
                        (o, i) =>
                        {
                            Presentation arrayElementPresentation = parameter.FortInspector.GetResolver().Resolve(new PresentationResolverParameter(parameter.DataType.GetElementType(),o, new PresentationSite
                            {
                                Base = parameter.Instance,
                                BasePresentation = this,
                                BaseSite = parameter.PresentationSite,
                                SiteType = PresentationSiteType.ArrayElement
                            }) );
                            return arrayElementPresentation;
                        })
                    .ToArray();
            }
        }
        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            Change change = new Change();
            object presentationData = parameter.PresentationData;
            if (presentationData != null && !((presentationData) is ArrayPresentationData))
                presentationData = null;
            ArrayPresentationData arrayPresentationData;
            if (presentationData != null)
            {
                arrayPresentationData = (ArrayPresentationData)presentationData;
                _isFoldout = arrayPresentationData.IsFoldout;
            }
            else
            {
                arrayPresentationData = new ArrayPresentationData();
            }
            arrayPresentationData.IsFoldout = EditorGUILayout.Foldout(_isFoldout, parameter.Title);
            change.IsPresentationChanged |= arrayPresentationData.IsFoldout != _isFoldout;
            _isFoldout = arrayPresentationData.IsFoldout;
            EditorGUILayout.BeginHorizontal();
            GUILayoutUtility.GetRect(3f, 6f);
            object data = parameter.Instance;
            if (data == null)
            {
                data = Array.CreateInstance(parameter.DataType.GetElementType(), 0);
                change.IsDataChanged = true;
            }
            Array arrayData = (Array) data;
            Initialize(arrayData,parameter);
            if (_isFoldout)
            {
                EditorGUILayout.BeginVertical();
                GUI.SetNextControlName(_sizeControlName);
                _elementSize = EditorGUILayout.IntField("Size", _elementSize);
                //Debug.Log(_elementSize);
                Event e = Event.current;
                if (e.keyCode == KeyCode.Return || GUI.GetNameOfFocusedControl() != _sizeControlName)
                {
                    if (arrayData.Length != _elementSize)
                    {
                        Array newArray = Array.CreateInstance(parameter.DataType.GetElementType(), _elementSize);
                        Presentation[] arrayElementPresentations = _arrayElementPresentations;
                        _arrayElementPresentations = new Presentation[_elementSize];
                        for (int i = 0; i < newArray.Length; i++)
                        {
                            if (i >= arrayData.Length)
                            {
                                newArray.SetValue(CreateNewElement(parameter,i),i);
                            }
                            else
                            {
                                newArray.SetValue(arrayData.GetValue(i), i);
                            }
                        }
                        arrayData = newArray;

                        for (int i = 0; i < _arrayElementPresentations.Length; i++)
                        {
                            if (i >= arrayElementPresentations.Length)
                            {
                                _arrayElementPresentations[i] = parameter.FortInspector.GetResolver().Resolve(new PresentationResolverParameter(parameter.DataType.GetElementType(), arrayData.GetValue(i), new PresentationSite
                                {
                                    Base = parameter.Instance,
                                    BasePresentation = this,
                                    BaseSite = parameter.PresentationSite,
                                    SiteType = PresentationSiteType.ArrayElement
                                }));
/*
                                PresentationParamater elementInitializationParameter =
                                new PresentationParamater(null,null,
                                    string.Empty, parameter.DataType.GetElementType(),
                                    new PresentationSite
                                    {
                                        Base = parameter.Instance,
                                        BasePresentation = this,
                                        BaseSite = parameter.PresentationSite,
                                        SiteType = PresentationSiteType.ArrayElement
                                    }, parameter.FortInspector);
*/

                                //_arrayElementPresentations[i].Initialize(elementInitializationParameter);
                            }
                            else
                            {
                                _arrayElementPresentations[i] = arrayElementPresentations[i];
                            }
                        }
                        change.IsDataChanged = true;
                    }
                }
                EditorGUILayout.BeginHorizontal();
                GUILayoutUtility.GetRect(3f, 6f);
                EditorGUILayout.BeginVertical();
                object[] elementPresentationData = new object[_arrayElementPresentations.Length];
                
                change.ChildrenChange = new Change[arrayData.Length];
                for (int i = 0; i < arrayData.Length; i++)
                {
                    PresentationParamater guiParameter =
                        new PresentationParamater(arrayData.GetValue(i),
                            arrayPresentationData.ElementPresentationData.Length > i
                                ? arrayPresentationData.ElementPresentationData[i]
                                : null,string.Format("Element {0}",i), parameter.DataType.GetElementType(),
                            new PresentationSite
                            {
                                Base = arrayData,
                                BasePresentation = this,
                                BaseSite = parameter.PresentationSite,
                                SiteType = PresentationSiteType.ArrayElement
                            }, parameter.FortInspector);

                    PresentationResult presentationResult =
                        _arrayElementPresentations[i].OnInspectorGui(guiParameter);
                    arrayData.SetValue(presentationResult.Result,i);
                    change.ChildrenChange[i] = presentationResult.Change;                    
                    elementPresentationData[i] = presentationResult.PresentationData;
                }
                arrayPresentationData.ElementPresentationData = elementPresentationData;
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndHorizontal();
            return new PresentationResult
            {
                Result = arrayData,
                Change = change,
                PresentationData = arrayPresentationData
            };
        }

        protected virtual object CreateNewElement(PresentationParamater paramater, int index)
        {
            if (paramater.DataType.GetElementType().IsAbstract)
            {
                return null;
            }
            if (paramater.DataType.GetElementType() == typeof (string))
            {
                return string.Empty;
            }
            return Activator.CreateInstance(paramater.DataType.GetElementType());
        }

        #endregion
        [Serializable]
        class ArrayPresentationData
        {
            public ArrayPresentationData()
            {
                ElementPresentationData = new object[0];
            }
            public bool IsFoldout { get; set; }
            public object[] ElementPresentationData { get; set; }
        }
    }
}
