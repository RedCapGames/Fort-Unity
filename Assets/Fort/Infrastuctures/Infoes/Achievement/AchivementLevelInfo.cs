using System;
using Fort.Inspector;

namespace Fort.Info.Achievement
{
    [Serializable]
    public class AchievementLevelInfo
    {
        public AchievementLevelInfo()
        {
            Id = Guid.NewGuid().ToString();
        }
        [IgnoreProperty]
        public string Id { get; set; }
        public int Score { get; set; }
        public Balance Balance { get; set; }
    }
}