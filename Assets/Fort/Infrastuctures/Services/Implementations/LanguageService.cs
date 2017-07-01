using System.Linq;
using Assets.Fort.Infrastuctures.Events;
using Fort.Info;
using Fort.Info.Language;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(ILanguageService))]
    class LanguageService:MonoBehaviour,ILanguageService
    {
        #region Implementation of ILanguageService

        public void ActivateLanguage(LanguageInfo language)
        {
            LanguageSavedData languageSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<LanguageSavedData>() ?? new LanguageSavedData();
            languageSavedData.LanguageId = language.Id;
            ServiceLocator.Resolve<IStorageService>().UpdateData(languageSavedData);
            ServiceLocator.Resolve<IEventAggregatorService>().GetEvent<LanguageActivatedEvent>().Publish(new LanguageActivatedEventArgs(language));
        }

        public LanguageInfo GetActiveLanguage()
        {
            LanguageSavedData languageSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<LanguageSavedData>() ?? new LanguageSavedData();
            LanguageInfo result =
                InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.Where(info => info != null).FirstOrDefault(
                    info => info.Id == languageSavedData.LanguageId);
            if (result == null)
            {
                result = InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage;
                if (result == null)
                {
                    result = InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.FirstOrDefault(info => info != null);
                }
            }
            return result;
        }

        public LanguageInfo[] GetLanguagesList()
        {
            return InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.Where(info => info != null).ToArray();
        }

        #endregion

        class LanguageSavedData
        {
            public string LanguageId { get; set; }
        }
    }
}
