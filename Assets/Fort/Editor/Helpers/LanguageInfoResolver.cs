using System.IO;
using Fort.Info;
using Fort.Info.Language;
using UnityEditor;
using UnityEngine;

namespace Fort
{
    public static class LanguageInfoResolver
    {
        private const string LanguageSettingAssetName = "LanguageInfo";
        private const string LanguageSettingPath = "Fort/Editor";
        internal static void UpdateLanguageEditorInfo(LanguageEditorInfo languageEditorInfo)
        {
            _languageEditorInfo = languageEditorInfo;
        }
        private static LanguageEditorInfo _languageEditorInfo;
        public static LanguageEditorInfo LanguageEditorInfo
        {
            get
            {
                if (_languageEditorInfo != null)
                    return _languageEditorInfo;
                LanguageScriptableObject languageScriptableObject =
                    AssetDatabase.LoadAssetAtPath<LanguageScriptableObject>("Assets/Fort/Editor/LanguageInfo.asset");
                if(languageScriptableObject == null)
                    return _languageEditorInfo = new LanguageEditorInfo();
                LanguageEditorInfo languageEditorInfo = (LanguageEditorInfo)languageScriptableObject.Load(typeof(LanguageEditorInfo));
                return _languageEditorInfo = languageEditorInfo;
            }
        }

        public static void Save(this LanguageEditorInfo languageEditorInfo)
        {
            LanguageScriptableObject languageScriptableObject =
                    AssetDatabase.LoadAssetAtPath<LanguageScriptableObject>("Assets/Fort/Editor/LanguageInfo.asset");
            bool newCreation = false;
            if (languageScriptableObject == null)
            {
                string fullPath = Path.Combine(Path.Combine("Assets", LanguageSettingPath),
                    LanguageSettingAssetName + ".asset");
                languageScriptableObject = ScriptableObject.CreateInstance<LanguageScriptableObject>();
                AssetDatabase.CreateAsset(languageScriptableObject, fullPath);
                newCreation = true;
            }
            if (!newCreation)
            {
                languageScriptableObject.Save(languageEditorInfo);
                EditorUtility.SetDirty(languageScriptableObject);
            }
        }

        public static void SyncFortAndSave(this LanguageEditorInfo languageEditorInfo,bool saveFort)
        {
            foreach (LanguageInfo languageInfo in languageEditorInfo.Languages)
            {
                for (int i = 0; i < InfoResolver.FortInfo.Language.ActiveLanguages.Length; i++)
                {
                    if (InfoResolver.FortInfo.Language.ActiveLanguages[i].Id == languageInfo.Id)
                    {
                        InfoResolver.FortInfo.Language.ActiveLanguages[i] = languageInfo;
                    }
                }
                if (InfoResolver.FortInfo.Language.DefaultLanguage.Id == languageInfo.Id)
                    InfoResolver.FortInfo.Language.DefaultLanguage = languageInfo;
            }
            if(saveFort)
                InfoResolver.FortInfo.Save();
            languageEditorInfo.Save();
        }
        [MenuItem("Fort/Settings/Language defenitions")]
        public static void ShowFortSettings()
        {
            LanguageScriptableObject languageScriptableObject =
        AssetDatabase.LoadAssetAtPath<LanguageScriptableObject>("Assets/Fort/Editor/LanguageInfo.asset");
            if (languageScriptableObject == null)
            {
                LanguageEditorInfo.Save();
                languageScriptableObject =
                    AssetDatabase.LoadAssetAtPath<LanguageScriptableObject>("Assets/Fort/Editor/LanguageInfo.asset");
            }
            Selection.activeObject = languageScriptableObject;
        }
    }
}
