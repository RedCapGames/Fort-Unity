using Fort.Info;
using Fort.Inspector;
using UnityEditor;

namespace FortTapsell.Info
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TapsellInfoScriptableObject), true)]
    public class TapsellInfoEditor : FortInspector
    {
        [MenuItem("Fort/Settings/Tapsell/Global Configuarion")]
        public static void ShowSetting()
        {
            EditorInfoResolver.ShowInfo<TapsellInfo>();
        }
    }


}
