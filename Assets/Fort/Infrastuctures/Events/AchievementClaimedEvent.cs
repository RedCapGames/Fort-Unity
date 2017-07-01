using System;
using Fort.Aggregator;
using Fort.Info.Achievement;

namespace Fort.Events
{
    public class AchievementClaimedEvent:PubSubEvent<AchievementClaimedEventArgs>
    {
    }

    public abstract class AchievementClaimedEventArgs : EventArgs
    {
        protected AchievementClaimedEventArgs(AchievementInfo achievementInfo)
        {
            AchievementInfo = achievementInfo;
        }

        public AchievementInfo AchievementInfo { get; private set; }
    }

    public class LevelBaseAchievementClaimedEventArgs : AchievementClaimedEventArgs
    {
        public LevelBaseAchievementInfo LevelBaseAchievementInfo { get; private set; }
        public int AchievementLevelIndex { get; private set; }


        public LevelBaseAchievementClaimedEventArgs(LevelBaseAchievementInfo achievementInfo,int achievementLevelIndex) : base(achievementInfo)
        {
            LevelBaseAchievementInfo = achievementInfo;
            AchievementLevelIndex = achievementLevelIndex;
        }
    }
    public class NoneLeveBaseAchievementClaimedEventArgs: AchievementClaimedEventArgs
    {
        public NoneLevelBaseAchievementInfo NoneLevelBaseAchievementInfo { get; private set; }

        public NoneLeveBaseAchievementClaimedEventArgs(NoneLevelBaseAchievementInfo achievementInfo) : base(achievementInfo)
        {
            NoneLevelBaseAchievementInfo = achievementInfo;
            
        }
    }
}
