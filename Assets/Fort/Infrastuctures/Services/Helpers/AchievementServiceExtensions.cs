using System;
using Fort.Info;

namespace Fort
{
    public static class AchievementServiceExtensions
    {
        public static void ClaimAchievement<T>(this IAchievementService achievementService)
            where T : NoneLevelBaseAchievementInfo
        {
            achievementService.ClaimAchievement(typeof(T));
        }
        public static void ClaimAchievement<T>(this IAchievementService achievementService, int achivementLevelIndex)
            where T : LevelBaseAchievementInfo
        {
            achievementService.ClaimAchievement(typeof(T),achivementLevelIndex);
        }
        public static void ClaimAchievement(this NoneLevelBaseAchievementInfo noneLevelBaseAchievementInfo)
        {
            ServiceLocator.Resolve<IAchievementService>().ClaimAchievement(noneLevelBaseAchievementInfo.GetType());
        }
        public static void ClaimAchievement(this LevelBaseAchievementInfo levelBaseAchievementInfo,
            int achivementLevelIndex)
        {
            ServiceLocator.Resolve<IAchievementService>().ClaimAchievement(levelBaseAchievementInfo.GetType(),achivementLevelIndex);
        }
        public static void ClaimAchievement(this AchievementLevelInfo achievementLevelInfo)
        {
            ServiceLocator.Resolve<IAchievementService>().ClaimAchievement(InfoResolver.FortInfo.Achievement.AchievementTokens[achievementLevelInfo.Id].AchievementInfo.GetType(), InfoResolver.FortInfo.Achievement.AchievementTokens[achievementLevelInfo.Id].Index);
        }
        public static bool IsAchievementClaimed<T>(this IAchievementService achievementService)
            where T : NoneLevelBaseAchievementInfo
        {
            return achievementService.IsAchievementClaimed(typeof (T));
        }
        public static AchievementLevelInfo GetNextClaimableAchievementLevelInfo<T>(this IAchievementService achievementService)
            where T: LevelBaseAchievementInfo
        {
            return achievementService.GetNextClaimableAchievementLevelInfo(typeof (T));
        }
        public static AchievementLevelInfo GetNextClaimableAchievementLevelInfo(this IAchievementService achievementService, Type levelBaseType)
        {
            int index = achievementService.GetAchievementClaimedIndex(levelBaseType)+1;
            AchievementInfo achievementInfo = InfoResolver.FortInfo.Achievement.AchievementTypes[levelBaseType];
            Array value = (Array)levelBaseType.GetProperty("LevelInfo").GetValue(achievementInfo,new object[0]);
            if(index>= value.Length)
                return null;
            return (AchievementLevelInfo) value.GetValue(index);
        }
        public static T  GetNextClaimableAchievementLevelInfo<T>(this LevelBaseAchievementInfo<T> levelBaseAchievementInfo)
            where T : AchievementLevelInfo
        {
            return (T) ServiceLocator.Resolve<IAchievementService>().GetNextClaimableAchievementLevelInfo(levelBaseAchievementInfo.GetType());
        }
        public static int GetAchievementClaimedIndex<T>(this IAchievementService achievementService)
            where T : LevelBaseAchievementInfo
        {
            return achievementService.GetAchievementClaimedIndex(typeof(T));
        }
        public static bool IsAchievementClaimed(this AchievementLevelInfo achievementLevelInfo)
        {
            AchievementToken achievementToken = InfoResolver.FortInfo.Achievement.AchievementTokens[achievementLevelInfo.Id];
            int achievementClaimedIndex = ServiceLocator.Resolve<IAchievementService>().GetAchievementClaimedIndex(achievementToken.AchievementInfo.GetType());
            return achievementToken.Index <= achievementClaimedIndex;            
        }
        public static ScoreBalance GetRealScoreBalance(this NoneLevelBaseAchievementInfo noneLevelBaseAchievement)
        {
            return
                ServiceLocator.Resolve<IAchievementService>()
                    .ResolveAchievementScoreBalance(noneLevelBaseAchievement.Id);
        }
        public static ScoreBalance GetRealScoreBalance(this AchievementLevelInfo achievementLevelInfo)
        {
            return
                ServiceLocator.Resolve<IAchievementService>()
                    .ResolveAchievementScoreBalance(achievementLevelInfo.Id);

        }        
    }
}