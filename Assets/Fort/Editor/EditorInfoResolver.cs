using System;
using System.Collections.Generic;
using System.IO;
using Fort.Info.Language;
using Fort.Inspector;
using UnityEditor;
using UnityEngine;

namespace Fort.Info
{
    public static class EditorInfoResolver
    {
        public static bool GetLoadingSequence(Type infoType)
        {
            if (!typeof(IInfo).IsAssignableFrom(infoType))
                throw new Exception("Only IInfo inherited interface types can be resolved");
            if (LoadingSequences.ContainsKey(infoType))
                return LoadingSequences[infoType];
            return false;
        }

        public static bool GetLoadingSequence<T>() where T : IInfo
        {
            return GetLoadingSequence(typeof(T));
        }

        private static readonly Dictionary<Type, IInfo> Infoes = new Dictionary<Type, IInfo>();
        private static readonly Dictionary<Type, bool> LoadingSequences = new Dictionary<Type, bool>();

        public static void UpdateInfo(Type infoType, IInfo info)
        {
            Infoes[infoType] = info;
        }

        public static string GetInfoLocation(Type type)
        {
            if (typeof (LanguageEditorInfo) == type)
                return "Assets/Fort/Editor/LanguageInfo.asset";
            InfoAttribute infoAttribute = type.GetCustomAttribute<InfoAttribute>();
            if (infoAttribute == null)
                throw new Exception("Info attribute not found");
            return string.Format("Assets/Fort/Editor/{0}/{1}.asset", infoAttribute.PluginName, type.Name);
        }

        public static IInfo Resolve(Type infoType)
        {
            if (!typeof(IInfo).IsAssignableFrom(infoType))
                throw new Exception("Only IInfo inherited interface types can be resolved");
            InfoAttribute infoAttribute = infoType.GetCustomAttribute<InfoAttribute>();
            if (infoAttribute == null)
                throw new Exception("Info attribute not found");
            if (!infoAttribute.Editor)
                return InfoResolver.Resolve(infoType);
            if (Infoes.ContainsKey(infoType))
                return Infoes[infoType];
            FortScriptableObject fortScriptableObject = AssetDatabase.LoadAssetAtPath<FortScriptableObject>(GetInfoLocation(infoType));
            if (fortScriptableObject == null)
            {
                IInfo result = (IInfo) Activator.CreateInstance(infoType);
                Infoes[infoType] = result;
                return result;
            }
            LoadingSequences[infoType] = true;
            IInfo info = fortScriptableObject.Load(infoType);
            Infoes[infoType] = info;
            LoadingSequences[infoType] = false;
            return info;
        }

        public static T Resolve<T>() where T : IInfo
        {
            return (T)Resolve(typeof(T));
        }

        public static void Save(Type infoType, IInfo info)
        {
            InfoAttribute infoAttribute = infoType.GetCustomAttribute<InfoAttribute>();
            if (infoAttribute == null)
                throw new Exception("Info attribute not found");
            if (infoAttribute.Editor)
            {
                string infoLocation = GetInfoLocation(infoType);
                string resourceDirectoryName = Path.GetDirectoryName(infoLocation);
                AssetDatabaseHelper.CreateFolderRecursive(resourceDirectoryName);
                FortScriptableObject fortScriptableObject =
                    AssetDatabase.LoadAssetAtPath<FortScriptableObject>(infoLocation);
                bool newCreation = false;
                if (fortScriptableObject == null)
                {

                    fortScriptableObject = (FortScriptableObject) ScriptableObject.CreateInstance(infoAttribute.ScriptableType);
                    AssetDatabase.CreateAsset(fortScriptableObject, infoLocation);
                    newCreation = true;
                }
                if (!newCreation)
                {
                    fortScriptableObject.Save(info);
                    EditorUtility.SetDirty(fortScriptableObject);
                }
            }
            else
            {
                FortScriptableObject fortInfoScriptable = Resources.Load<FortScriptableObject>(InfoResolver.GetInfoResourceRelativeLocation(infoType));
                bool newCreation = false;

                if (fortInfoScriptable == null)
                {
                    string infoResourceFullLocation = InfoResolver.GetInfoResourceFullLocation(infoType);
                    string resourceDirectoryName = Path.GetDirectoryName(infoResourceFullLocation);
                    AssetDatabaseHelper.CreateFolderRecursive(resourceDirectoryName);

                    fortInfoScriptable = (FortScriptableObject) ScriptableObject.CreateInstance(infoAttribute.ScriptableType);
                    AssetDatabase.CreateAsset(fortInfoScriptable, infoResourceFullLocation);
                    newCreation = true;
                }
                if (!newCreation)
                {
                    fortInfoScriptable.Save(info);
                    EditorUtility.SetDirty(fortInfoScriptable);
                }
            }
        }

        public static void Save<T>(this T info) where T : IInfo
        {
            if(info == null)
                return;
            Save(info.GetType(),info);
        }
        
        public static void ShowInfo(Type infoType)
        {
            InfoAttribute infoAttribute = infoType.GetCustomAttribute<InfoAttribute>();
            if (infoAttribute == null)
                throw new Exception("Info attribute not found");
            if (infoAttribute.Editor)
            {
                FortScriptableObject fortScriptableObject =
                    AssetDatabase.LoadAssetAtPath<FortScriptableObject>(GetInfoLocation(infoType));
                if (fortScriptableObject == null)
                {
                    Resolve(infoType).Save();
                    fortScriptableObject =
                        AssetDatabase.LoadAssetAtPath<FortScriptableObject>(GetInfoLocation(infoType));
                }
                Selection.activeObject = fortScriptableObject;
            }
            else
            {
                FortScriptableObject fortScriptableObject = Resources.Load<FortScriptableObject>(InfoResolver.GetInfoResourceRelativeLocation(infoType));
                if (fortScriptableObject == null)
                {
                    InfoResolver.Resolve(infoType).Save();
                    fortScriptableObject = Resources.Load<FortScriptableObject>(InfoResolver.GetInfoResourceRelativeLocation(infoType));
                }
                Selection.activeObject = fortScriptableObject;
            }
        }

        public static void ShowInfo<T>() where T : IInfo
        {
            ShowInfo(typeof(T));
        }
    }
}
