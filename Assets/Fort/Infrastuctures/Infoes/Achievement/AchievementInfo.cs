using System;
using Fort.Inspector;

namespace Fort.Info.Achievement
{
    [Serializable]
    public abstract class AchievementInfo
    {
        protected AchievementInfo()
        {
            Id = Guid.NewGuid().ToString();
        }

        [IgnorePresentation]
        public string Id { get; set; }
        public string Name { get { return GetType().Name; } }
        public string DisplayName { get; set; }
    }
}