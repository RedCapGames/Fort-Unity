using System;
using System.Collections.Generic;
using Fort.Inspector;
using UnityEngine;

namespace Fort.Info
{
    public static class InfoResolver
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
            return GetLoadingSequence(typeof (T));
        }

        private static readonly Dictionary<Type, IInfo> Infoes = new Dictionary<Type, IInfo>();
        private static readonly Dictionary<Type, bool> LoadingSequences = new Dictionary<Type, bool>();

#if UNITY_EDITOR
        public static void UpdateInfo(Type infoType,IInfo info)
        {
            Infoes[infoType] = info;
        }
#else
        internal static void UpdateInfo(Type infoType, IInfo info)
        {
            Infoes[infoType] = info;
        }

#endif

        public static string GetInfoResourceFullLocation(Type type)
        {
            if (type == typeof (FortInfo))
                return "Assets/Fort/Resources/FortInfo.asset";
            InfoAttribute infoAttribute = type.GetCustomAttribute<InfoAttribute>();
            if (infoAttribute == null)
                throw new Exception("Info attribute not found");
            return string.Format("Assets/Fort/Resources/{0}/{1}.asset",infoAttribute.PluginName,type.Name);
        }
        public static string GetInfoResourceRelativeLocation(Type type)
        {
            if (type == typeof(FortInfo))
                return "FortInfo";
            InfoAttribute infoAttribute = type.GetCustomAttribute<InfoAttribute>();
            if (infoAttribute == null)
                throw new Exception("Info attribute not found");
            return string.Format("{0}/{1}", infoAttribute.PluginName, type.Name);
        }

        public static IInfo Resolve(Type infoType)
        {
            if(!typeof(IInfo).IsAssignableFrom(infoType))
                throw new Exception("Only IInfo inherited interface types can be resolved");
            InfoAttribute infoAttribute = infoType.GetCustomAttribute<InfoAttribute>();
            if(infoAttribute == null)
                throw new Exception("Info attribute not found");
            if(infoAttribute.Editor)
                throw new Exception("Only None editor info can be resolve from Info resolver.Use EditorInfoResolver instead");
            if (Infoes.ContainsKey(infoType))
                return Infoes[infoType];
            FortScriptableObject fortScriptableObject = (FortScriptableObject) Resources.Load(GetInfoResourceRelativeLocation(infoType));
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
            return (T) Resolve(typeof (T));
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class InfoAttribute : Attribute
    {
        public Type ScriptableType { get; private set; }
        public string PluginName { get; private set; }
        public bool Editor { get; private set; }

        public InfoAttribute(Type scriptableType,string pluginName,bool editor)
        {
            ScriptableType = scriptableType;
            PluginName = pluginName;
            Editor = editor;
        }
    }
}