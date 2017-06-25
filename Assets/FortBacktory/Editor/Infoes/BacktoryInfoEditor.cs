using Fort.Info;
using Fort.Inspector;
using UnityEditor;

namespace FortBacktory.Info
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BacktoryInfoScriptableObject), true)]
    public class BacktoryInfoEditor:FortInspector
    {
        [MenuItem("Fort/Bactory/Settings/Global Configuarion")]
        public static void ShowSetting()
        {
            EditorInfoResolver.ShowInfo<BacktoryInfo>();
        }
    }
}
