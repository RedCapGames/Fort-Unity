using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Info;
using Fort.Info.Language;
using UnityEditor;

namespace Fort
{
    public static class LanguageHelper
    {
        public static void SyncFortAndSave(this LanguageEditorInfo languageEditorInfo, bool saveFort)
        {
            foreach (LanguageInfo languageInfo in languageEditorInfo.Languages)
            {
                for (int i = 0; i < InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.Length; i++)
                {
                    if (InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages[i].Id == languageInfo.Id)
                    {
                        InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages[i] = languageInfo;
                    }
                }
                if (InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage != null && InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage.Id == languageInfo.Id)
                    InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage = languageInfo;
            }
            if (saveFort)
                InfoResolver.Resolve<FortInfo>().Save();
            languageEditorInfo.Save();
        }

        [MenuItem("Fort/Settings/Language defenitions")]
        public static void ShowLanguageSettings()
        {
            EditorInfoResolver.ShowInfo<LanguageEditorInfo>();
        }
    }
}
