using Fort.Info;
using Fort.Inspector;
using UnityEditor;

namespace FortBacktory.Info
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BacktoryEditorInfoScriptableObject), true)]
    public class BacktoryEditorInfoEditor:FortInspector
    {
        [MenuItem("Fort/Settings/Bactory/Editor Configuarion")]
        public static void ShowSetting()
        {
            EditorInfoResolver.ShowInfo<BacktoryEditorInfo>();
        }

    }
}
