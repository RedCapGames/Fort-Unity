using System;
using System.Collections.Generic;
using System.Reflection;

namespace Fort.Info.Achievement
{
    [Serializable]
    public abstract class NoneLevelBaseAchievementInfo : AchievementInfo
    {
        public int Score { get; set; }
        public Balance Balance { get; set; }
        public string Link { get; set; }
        public bool ClaimAchievementOnClick { get; set; }
        public string GetDisplayName()
        {
            if (string.IsNullOrEmpty(DisplayName))
                return string.Empty;
            string displayName = DisplayName;
            foreach (FieldInfo fieldInfo in GetType().GetFields())
            {
                displayName = displayName.Replace("{" + fieldInfo.Name + "}",
                    fieldInfo.GetValue(this).ToString());
            }
            return displayName;
        }
    }
}