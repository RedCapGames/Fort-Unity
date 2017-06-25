using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Info;
using Fort.Info.Language;
using Fort.Inspector;
using UnityEditor;

namespace FortBacktory.Info
{
    [CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(BacktoryEditorInfoScriptableObject), true)]
    public class BacktoryEditorInfoEditor:FortInspector
    {
        [MenuItem("Fort/Bactory/Settings/Editor Configuarion")]
        public static void ShowSetting()
        {
            EditorInfoResolver.ShowInfo<BacktoryEditorInfo>();
        }

    }
}
