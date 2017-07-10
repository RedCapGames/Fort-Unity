using Fort.Info;
using Fort.Inspector;
using UnityEditor;

namespace FortTapligh.Info
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TaplighInfoScriptableObject), true)]
    public class TaplighInfoEditor : FortInspector
    {
        [MenuItem("Fort/Settings/Tapligh/Global Configuarion")]
        public static void ShowSetting()
        {
            EditorInfoResolver.ShowInfo<TaplighInfo>();
        }

    }
}

