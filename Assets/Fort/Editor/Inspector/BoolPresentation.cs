using UnityEditor;

namespace Fort.Inspector
{
    class BoolPresentation : Presentation
    {

        #region Overrides of PresentationFieldInfo

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            bool result = EditorGUILayout.Toggle(parameter.Title, (bool)parameter.Instance);
            return new PresentationResult
            {
                Result = result,
                Change = new Change { IsDataChanged = ((bool)parameter.Instance) != result } 
            };
        }

        #endregion
    }
}