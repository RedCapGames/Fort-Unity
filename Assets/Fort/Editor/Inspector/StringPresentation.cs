using System;
using System.Reflection;
using Mono.CSharp;
using UnityEditor;

namespace Fort.Inspector
{
    class StringPresentation : Presentation
    {

        #region Overrides of PresentationFieldInfo
        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            string result = EditorGUILayout.TextField(parameter.Title, (string)parameter.Instance);
            return new PresentationResult
            {
                Result = result,
                Change = new Change { IsDataChanged = result != (string)parameter.Instance }                
            };
        }

        #endregion
    }
}