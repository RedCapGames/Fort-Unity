using System.IO;
using Fort.Info;
using Fort.Inspector;
using UnityEditor;
using UnityEngine;

namespace Fort
{
    public static class FortInfoEditorExtentions
    {
        #region Fields

        private const string FortSettingAssetName = "FortInfo";
        private const string FortSettingPath = "Fort/Resources";

        #endregion

        #region  Public Methods

        internal static void Save(this FortInfo fortInfo)
        {
            FortInfoScriptable fortInfoScriptable = Resources.Load<FortInfoScriptable>("FortInfo");
            bool newCreation = false;
            if (fortInfoScriptable == null)
            {
                string properPath = Path.Combine(Application.dataPath, FortSettingPath);
                if (!Directory.Exists(properPath))
                {
                    AssetDatabase.CreateFolder("Assets/Fort", "Resources");
                }
                string fullPath = Path.Combine(Path.Combine("Assets", FortSettingPath),
                    FortSettingAssetName + ".asset");
                fortInfoScriptable = ScriptableObject.CreateInstance<FortInfoScriptable>();
                AssetDatabase.CreateAsset(fortInfoScriptable, fullPath);
                newCreation = true;
            }
            if (!newCreation)
            {
                fortInfoScriptable.Save(fortInfo);
                EditorUtility.SetDirty(fortInfoScriptable);
            }
        }

        [MenuItem("Fort/Settings/Global Settings")]
        public static void ShowFortSettings()
        {
            FortInfoScriptable fortInfoScriptable = Resources.Load<FortInfoScriptable>("FortInfo");
            if (fortInfoScriptable == null)
            {
                InfoResolver.FortInfo.Save();
                fortInfoScriptable = Resources.Load<FortInfoScriptable>("FortInfo");
            }
            Selection.activeObject = fortInfoScriptable;
        }

        #endregion
    }
}