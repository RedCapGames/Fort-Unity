using Fort.Info.Language;

namespace Fort
{
    public interface ILanguageService
    {
        void ActivateLanguage(LanguageInfo language);
        LanguageInfo GetActiveLanguage();
        LanguageInfo[] GetLanguagesList();
    }
}
