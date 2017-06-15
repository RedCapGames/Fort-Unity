using Fort.Inspector;
using UnityEditor;

namespace Fort.Info
{
    [CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(FortInfoScriptable), true)]

    public class FortInfoEditor : FortInspector
    {
        #region Overrides of FortInspector

        protected override void OnTargetChanged(object targetObject)
        {
            InfoResolver.UpdateFortInfo((FortInfo) targetObject);
        }

        #endregion
    }
}
