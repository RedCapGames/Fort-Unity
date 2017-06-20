using Fort.Inspector;
using UnityEngine;

namespace Fort.Info
{
    public static class InfoResolver
    {
#if UNITY_EDITOR
        public static void UpdateFortInfo(FortInfo fortInfo)
        {
            _fortInfo = fortInfo;
        }
#else
        internal static void UpdateFortInfo(FortInfo fortInfo)
        {
            _fortInfo = fortInfo;
        }

#endif
        public static bool LoadingSequence { get; private set; }
        private static FortInfo _fortInfo;

        public static FortInfo FortInfo
        {
            get
            {
                if (_fortInfo != null)
                    return _fortInfo;
                FortInfoScriptable fortInfoScriptable = Resources.Load<FortInfoScriptable>("FortInfo");
                if(fortInfoScriptable == null)
                    return _fortInfo = new FortInfo();
                LoadingSequence = true;
                _fortInfo = (FortInfo)fortInfoScriptable.Load(typeof(FortInfo));
                LoadingSequence = false;
                return _fortInfo;
            }
        }

    }
}