using System.Linq;
using Fort.Info.Language;
using Fort.Inspector;
using UnityEditor;

namespace Fort.Info
{
    [CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(LanguageScriptableObject), true)]
    public class LanguageInfoEditor : FortInspector
    {
        #region Overrides of FortInspector

        protected override void OnTargetChanged(object targetObject)
        {
            LanguageEditorInfo languageEditorInfo = (LanguageEditorInfo)targetObject;
            LanguageInfoResolver.UpdateLanguageEditorInfo(languageEditorInfo);
            if (languageEditorInfo != null)
            {
                if (
                    InfoResolver.FortInfo.Language.ActiveLanguages.Any(
                        info => languageEditorInfo.Languages.All(languageInfo => languageInfo.Id != info.Id)))
                {
                    InfoResolver.FortInfo.Language.ActiveLanguages =
                        InfoResolver.FortInfo.Language.ActiveLanguages.Where(
                            info => languageEditorInfo.Languages.Any(languageInfo => languageInfo.Id == info.Id))
                            .ToArray();
                    InfoResolver.FortInfo.Save();
                }

            }

        }

        #endregion
    }
}
