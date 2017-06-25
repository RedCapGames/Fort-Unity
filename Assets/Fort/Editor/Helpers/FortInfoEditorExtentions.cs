using System.IO;
using Fort.Info;
using Fort.Inspector;
using UnityEditor;
using UnityEngine;

namespace Fort
{
    public static class FortInfoEditorExtentions
    {

        #region  Public Methods

        [MenuItem("Fort/Settings/Global Settings")]
        public static void ShowFortSettings()
        {
            EditorInfoResolver.ShowInfo<FortInfo>();
        }

        #endregion
    }
}