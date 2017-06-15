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
#endif
        private static FortInfo _fortInfo;

        public static FortInfo FortInfo
        {
            get { return _fortInfo = _fortInfo ?? (FortInfo)Resources.Load<FortInfoScriptable>("FortInfo").Load(typeof(FortInfo)); }
        }

    }
}