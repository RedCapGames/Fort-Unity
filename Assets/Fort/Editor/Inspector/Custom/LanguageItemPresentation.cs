using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fort.Info;
using Fort.Info.Language;
using Fort.Inspector;
using UnityEditor;
using UnityEngine;

namespace Fort.CustomEditor
{
    public class LanguageItemPresentation:Presentation
    {
        #region Overrides of Presentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {
            PresentationData presentationData = parameter.PresentationData as PresentationData;
            if(presentationData == null)
                presentationData = new PresentationData();
            LanguageItem languageItem = parameter.Instance as LanguageItem;
            Type itemType = parameter.DataType.GetProperty("Value").PropertyType;
            bool newLanguageItem = false;
            if (languageItem == null)
            {
                newLanguageItem = true;
                languageItem =
                    (LanguageItem) Activator.CreateInstance(typeof (InfoLanguageItem<>).MakeGenericType(itemType));
            }
            PresentationSite presentationSite = new PresentationSite
            {
                Base = languageItem,
                BaseSite = parameter.PresentationSite,
                BasePresentation = this,
                SiteType = PresentationSiteType.None
            };
            Change change;
            OverridableLanguageAttribute overridableLanguageAttribute = parameter.DataType.GetCustomAttribute<OverridableLanguageAttribute>();
            if (overridableLanguageAttribute == null)
            {
                PropertyInfo propertyInfo = parameter.PresentationSite.GetFirtTargetedPropertyInfo();
                if (propertyInfo != null)
                    overridableLanguageAttribute = propertyInfo.GetCustomAttribute<OverridableLanguageAttribute>();
            }
            if (overridableLanguageAttribute == null)
            {
                change = ApplyNotOvveriededLanguageItem(parameter, languageItem, itemType, presentationSite, presentationData);
            }
            else
            {
                if (newLanguageItem)
                {
                    ((IInfoLanguageItem) languageItem).UseOverridedValue = true;
                }
                change = ApplyOvveriededLanguageItem(parameter, languageItem, itemType, presentationSite, presentationData);
            }
            return new PresentationResult
            {
                Change = change,
                PresentationData = presentationData,
                Result = languageItem
            };
        }

        private static Change ApplyNotOvveriededLanguageItem(PresentationParamater parameter, LanguageItem languageItem,
            Type itemType, PresentationSite presentationSite, PresentationData presentationData)
        {
            GUIStyle guiStyle = new GUIStyle();
            Change change = new Change();
            List<Change> changes = new List<Change>();
            LanguageEditorInfo languageEditorInfo = EditorInfoResolver.Resolve<LanguageEditorInfo>();
            LanguageInfo[] languageInfos = languageEditorInfo.Languages.Where(info => info != null).ToArray();            
            if (languageInfos.Length == 1)
            {
                ApplyLanguagePrenetation(parameter, languageInfos[0], languageItem, itemType, presentationSite, presentationData,
                    changes, parameter.Title);
                Change last = changes.Last();
                if (last.IsAnyDataChanged())
                {
                    for (int i = 0; i < InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.Length; i++)
                    {
                        if (InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages[i].Id == languageInfos[0].Id)
                        {
                            InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages[i] = languageInfos[0];
                        }
                    }
                    if (InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage != null &&
                        InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage.Id == languageInfos[0].Id)
                    {
                        InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage = languageInfos[0];
                    }
                    languageEditorInfo.Save();
                    InfoResolver.Resolve<FortInfo>().Save();
                }
            }
            else
            {
                bool oldFoldout = presentationData.IsFoldout;
                presentationData.IsFoldout = EditorGUILayout.Foldout(presentationData.IsFoldout, parameter.Title);
                change.IsPresentationChanged = oldFoldout != presentationData.IsFoldout;
                if (presentationData.IsFoldout)
                {
                    EditorGUILayout.BeginHorizontal(guiStyle);
                    GUILayout.Space(FortInspector.ItemSpacing);
                    EditorGUILayout.BeginVertical(guiStyle);
                    foreach (LanguageInfo languageInfo in languageInfos)
                    {
                        ApplyLanguagePrenetation(parameter, languageInfo, languageItem, itemType, presentationSite,
                            presentationData, changes, languageInfo.Name);
                        Change last = changes.Last();
                        if (last.IsAnyDataChanged())
                        {
                            for (int i = 0; i < InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.Length; i++)
                            {
                                if (InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages[i].Id == languageInfo.Id)
                                {
                                    InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages[i] = languageInfo;
                                }
                            }
                            if (InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage != null &&
                                InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage.Id == languageInfo.Id)
                            {
                                InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage = languageInfo;
                            }
                        }
                    }
                    if (changes.Any(change1 => change1.IsAnyDataChanged()))
                    {
                        languageEditorInfo.Save();
                        InfoResolver.Resolve<FortInfo>().Save();
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
            }
            change.ChildrenChange = changes.ToArray();
            return change;
        }

        private static Change ApplyOvveriededLanguageItem(PresentationParamater parameter, LanguageItem languageItem,
    Type itemType, PresentationSite presentationSite, PresentationData presentationData)
        {
            GUIStyle guiStyle = new GUIStyle();
            Change change = new Change();
            List<Change> changes = new List<Change>();
            LanguageEditorInfo languageEditorInfo = EditorInfoResolver.Resolve<LanguageEditorInfo>();
            LanguageInfo[] languageInfos = languageEditorInfo.Languages.Where(info => info != null).ToArray();
            IInfoLanguageItem infoLanguageItem = (IInfoLanguageItem)languageItem;
            if (languageInfos.Length == 1)
            {
                
                if (!infoLanguageItem.UseOverridedValue)
                {
                    change.IsDataChanged = true;
                    //Remove Item From Languages
                    foreach (LanguageInfo languageInfo in languageEditorInfo.Languages)
                    {
                        if (languageInfo.LanguageDatas.ContainsKey(languageItem.Id))
                            languageInfo.LanguageDatas.Remove(languageItem.Id);
                    }
                }
                infoLanguageItem.UseOverridedValue = true;

                Presentation presentation =
                    parameter.FortInspector.GetResolver()
                        .Resolve(new PresentationResolverParameter(itemType, infoLanguageItem.GetOvverideValue(), presentationSite));
                PresentationResult presentationResult =
                    presentation.OnInspectorGui(new PresentationParamater(infoLanguageItem.GetOvverideValue(),
                        presentationData.OvverideItemPresentationData, parameter.Title, itemType, presentationSite,
                        parameter.FortInspector));
                infoLanguageItem.SetOvverideValue(presentationResult.Result);
                presentationData.OvverideItemPresentationData = presentationResult.PresentationData;
                change.ChildrenChange = new[] {presentationResult.Change};
                languageEditorInfo.SyncFortAndSave(false);
            }
            else
            {
                bool oldFoldout = presentationData.IsFoldout;
                presentationData.IsFoldout = EditorGUILayout.Foldout(presentationData.IsFoldout, parameter.Title);
                change.IsPresentationChanged = oldFoldout != presentationData.IsFoldout;
                if (presentationData.IsFoldout)
                {
                    EditorGUILayout.BeginHorizontal(guiStyle);
                    GUILayout.Space(FortInspector.ItemSpacing);
                    EditorGUILayout.BeginVertical(guiStyle);
                    bool oldUseOverridedValue = infoLanguageItem.UseOverridedValue;
                    infoLanguageItem.UseOverridedValue = EditorGUILayout.Toggle("Ovveride", infoLanguageItem.UseOverridedValue);
                    change.IsDataChanged |= oldUseOverridedValue != infoLanguageItem.UseOverridedValue;
                    if (infoLanguageItem.UseOverridedValue)
                    {
                        Presentation presentation =
                    parameter.FortInspector.GetResolver()
                        .Resolve(new PresentationResolverParameter(itemType, infoLanguageItem.GetOvverideValue(), presentationSite));
                        PresentationResult presentationResult =
                            presentation.OnInspectorGui(new PresentationParamater(infoLanguageItem.GetOvverideValue(),
                                presentationData.OvverideItemPresentationData, parameter.Title, itemType, presentationSite,
                                parameter.FortInspector));
                        infoLanguageItem.SetOvverideValue(presentationResult.Result);
                        presentationData.OvverideItemPresentationData = presentationResult.PresentationData;
                        change.ChildrenChange = new[] { presentationResult.Change };
                        languageEditorInfo.SyncFortAndSave(false);
                    }
                    else
                    {
                        infoLanguageItem.SetOvverideValue(itemType.GetDefault());
                        foreach (LanguageInfo languageInfo in languageInfos)
                        {
                            ApplyLanguagePrenetation(parameter, languageInfo, languageItem, itemType, presentationSite,
                                presentationData, changes, languageInfo.Name);
                            Change last = changes.Last();
                            if (last.IsAnyDataChanged())
                            {
                                for (int i = 0; i < InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.Length; i++)
                                {
                                    if (InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages[i].Id == languageInfo.Id)
                                    {
                                        InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages[i] = languageInfo;
                                    }
                                }
                                if (InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage != null &&
                                    InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage.Id == languageInfo.Id)
                                {
                                    InfoResolver.Resolve<FortInfo>().Language.DefaultLanguage = languageInfo;
                                }
                            }
                        }
                        if (changes.Any(change1 => change1.IsAnyDataChanged()))
                        {
                            languageEditorInfo.Save();
                            InfoResolver.Resolve<FortInfo>().Save();
                        }
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                change.ChildrenChange = changes.ToArray();
            }
            
            return change;
        }
        private static void ApplyLanguagePrenetation(PresentationParamater parameter, LanguageInfo languageInfo, LanguageItem languageItem, Type itemType, PresentationSite presentationSite, PresentationData presentationData, List<Change> changes, string title)
        {
            object data = null;
            if (languageInfo.LanguageDatas.ContainsKey(languageItem.Id))
                data = languageInfo.LanguageDatas[languageItem.Id];

            Presentation presentation =
                parameter.FortInspector.GetResolver()
                    .Resolve(new PresentationResolverParameter(itemType, data, presentationSite));
            object languagePresentationData = null;
            if (presentationData.LanguagePresentationData.ContainsKey(languageInfo.Id))
                languagePresentationData = presentationData.LanguagePresentationData[languageInfo.Id];
            PresentationResult presentationResult =
                presentation.OnInspectorGui(new PresentationParamater(data, languagePresentationData,
                    title, itemType, presentationSite, parameter.FortInspector));
            changes.Add(presentationResult.Change);
            languageInfo.LanguageDatas[languageItem.Id] = presentationResult.Result;
            presentationData.LanguagePresentationData[languageInfo.Id] = presentationResult.PresentationData;
        }

        #endregion

        private class PresentationData
        {

            public PresentationData()
            {
                LanguagePresentationData = new Dictionary<string, object>();
            }
            public Dictionary<string,object> LanguagePresentationData { get; set; }
            public object OvverideItemPresentationData { get; set; }
            public bool IsFoldout { get; set; }
        }
    }
}
