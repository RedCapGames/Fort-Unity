using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Fort.Inspector
{
    class UnityObjectPresentation: Presentation
    {
        #region Overrides of Presentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            Object result = EditorGUILayout.ObjectField(parameter.Title, (UnityEngine.Object)parameter.Instance, parameter.DataType,false);
            return new PresentationResult
            {
                Result = result,
                Change = new Change { IsDataChanged = ((Object)parameter.Instance) != result }
            };
        }

        #endregion
    }
}
