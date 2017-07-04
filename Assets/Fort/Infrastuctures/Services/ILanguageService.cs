using Fort.Info.Language;

namespace Fort
{
    /// <summary>
    /// The service that used to manage languages
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>
        /// Activating language
        /// </summary>
        /// <param name="language">Corresponding language</param>
        void ActivateLanguage(LanguageInfo language);
        /// <summary>
        /// Get active language
        /// </summary>
        /// <returns>Active language</returns>
        LanguageInfo GetActiveLanguage();
        /// <summary>
        /// Get avialable language in publish
        /// </summary>
        /// <returns>Language list</returns>
        LanguageInfo[] GetLanguagesList();
    }
}
