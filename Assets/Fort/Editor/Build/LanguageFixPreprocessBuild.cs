using System.Collections.Generic;
using System.Linq;
using Fort.Info;
using Fort.Info.Language;
using Fort.Inspector;
using UnityEditor;

//using UnityEditor.Build;

namespace Fort.Build
{
    public class LanguageFixPreprocessBuild//: IPreprocessBuild
    {
        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            LanguageItem[] languageItems = TypeHelper.FindType(InfoResolver.FortInfo,typeof(LanguageItem)).Cast<LanguageItem>().ToArray();
            List<string> removedItems = new List<string>();
            LanguageEditorInfo languageEditorInfo = LanguageInfoResolver.LanguageEditorInfo;
            foreach (LanguageInfo languageInfo in languageEditorInfo.Languages)
            {
                foreach (KeyValuePair<string, object> pair in languageInfo.LanguageDatas)
                {
                    if(languageItems.All(item => item.Id != pair.Key))
                        removedItems.Add(pair.Key);
                }
            }
            FortInfo fortInfo = InfoResolver.FortInfo;
            foreach (string removedItem in removedItems)
            {
                foreach (LanguageInfo activeLanguage in fortInfo.Language.ActiveLanguages)
                {
                    if (activeLanguage.LanguageDatas.ContainsKey(removedItem))
                        activeLanguage.LanguageDatas.Remove(removedItem);
                }
                foreach (LanguageInfo languageInfo in languageEditorInfo.Languages)
                {
                    if (languageInfo.LanguageDatas.ContainsKey(removedItem))
                        languageInfo.LanguageDatas.Remove(removedItem);

                }
            }
            fortInfo.Save();
            languageEditorInfo.Save();
            //Debug.Log("MyCustomBuildProcessor.OnPreprocessBuild for target " + target + " at path " + path);
        }
    }
}
