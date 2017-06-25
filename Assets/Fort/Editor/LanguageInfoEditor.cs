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

        protected override void OnTargetChanged(IInfo targetObject)
        {
            LanguageEditorInfo languageEditorInfo = (LanguageEditorInfo)targetObject;
            if (languageEditorInfo != null)
            {
                if (
                    InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.Any(
                        info => languageEditorInfo.Languages.All(languageInfo => languageInfo.Id != info.Id)))
                {
                    InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages =
                        InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.Where(
                            info => languageEditorInfo.Languages.Any(languageInfo => languageInfo.Id == info.Id))
                            .ToArray();
                    InfoResolver.Resolve<FortInfo>().Save();
                }

            }

        }

        #endregion
    }
}
