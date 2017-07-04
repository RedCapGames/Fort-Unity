using System;
using System.Linq;
using System.Reflection;
using Fort.Inspector;

namespace Fort.Info.Language
{
    public class FortLanguage
    {
        public FortLanguage()
        {
            try
            {
                LanguageInfo firstLanguage = ResolveLanguageEditorInfo().Languages.FirstOrDefault();
                if (firstLanguage == null)
                {
                    ActiveLanguages = new LanguageInfo[0];
                }
                else
                {
                    ActiveLanguages = new[] { firstLanguage };
                }

            }
            catch (Exception)
            {
                ActiveLanguages = new LanguageInfo[0];
            }
            
        }
        internal static LanguageEditorInfo ResolveLanguageEditorInfo()
        {
            Type languageInfoResolverType =
                TypeHelper.GetAllTypes(AllTypeCategory.Editor)
                    .Single(type => string.Format("{0}.{1}", type.Namespace, type.Name) == "Fort.Info.EditorInfoResolver");
            return
                (LanguageEditorInfo)
                    languageInfoResolverType.GetMethods().First(info => info.Name == "Resolve" && !info.IsGenericMethod).Invoke(null,new []{typeof(LanguageEditorInfo) });
        }
        [PropertyInstanceResolve(typeof(ActiveLanguagesPropertyInstanceResolver))]
        public LanguageInfo[] ActiveLanguages { get; set; }
        [PropertyInstanceResolve(typeof(DefaultLanguagePropertyInstanceResolver))]
        public LanguageInfo DefaultLanguage { get; set; }
    }
    public class ActiveLanguagesPropertyInstanceResolver : IPropertyInstanceResolver
    {
        #region Implementation of IPropertyInstanceResolver

        public InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property)
        {
            LanguageInfo[] languageInfos = (LanguageInfo[])data;
            if (languageInfos == null)
                languageInfos = new LanguageInfo[0];
            LanguageInfo[] editorLanguages = FortLanguage.ResolveLanguageEditorInfo()
                .Languages;
            LanguageInfo[] possibleLanguages =
                editorLanguages.Where(info => languageInfos.All(languageInfo => languageInfo.Id != info.Id))
                    .ToArray();
            InstanceResolverResult result = new InstanceResolverResult
            {
                PossibleInstanceTokens =
                    possibleLanguages.Select(language => new InstanceToken(language.Name, language)).ToArray(),
                PresentableInstanceTokens =
                    languageInfos.Where(info => editorLanguages.Any(languageInfo => languageInfo.Id == info.Id))
                        .Select(info => new InstanceToken(info.Name, info))
                        .ToArray(),
                IsEditable = false,
                UseValueTypeForEdit = false
            };
            return result;
        }

        #endregion
    }
    public class DefaultLanguagePropertyInstanceResolver : IPropertyInstanceResolver
    {
        #region Implementation of IPropertyInstanceResolver

        public InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property)
        {
            LanguageInfo[] editorLanguages = FortLanguage.ResolveLanguageEditorInfo().Languages;
            InstanceResolverResult result = new InstanceResolverResult
            {
                PossibleInstanceTokens =
                    InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.Where(
                        info => editorLanguages.Any(languageInfo => languageInfo.Id == info.Id))
                        .Select(info => new InstanceToken(info.Name, info))
                        .ToArray()
            };
            InstanceToken instanceToken =
                result.PossibleInstanceTokens.FirstOrDefault(token => ReferenceEquals(token.Value, data));
            result.PresentableInstanceTokens = instanceToken == null ? new InstanceToken[0] : new[] { instanceToken };
            return result;
        }

        #endregion
    }
}