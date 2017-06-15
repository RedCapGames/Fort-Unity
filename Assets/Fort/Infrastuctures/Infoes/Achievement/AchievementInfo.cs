using System;
using Fort.Inspector;

namespace Fort.Info
{
    [Serializable]
    public abstract class AchievementInfo
    {
        protected AchievementInfo()
        {
            Id = Guid.NewGuid().ToString();
        }

        [IgnoreProperty]
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}