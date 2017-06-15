using System;
using System.Collections.Generic;
using System.Linq;
using Fort.Info;

namespace Fort
{
    public interface IAchievementService
    {
        void ClaimAchievement(Type noneLevelBaseType);
        void ClaimAchievement(Type levelBaseType, int achivementLevelIndex);
        bool IsAchievementClaimed(Type noneLevelBaseType);
        int GetAchievementClaimedIndex(Type levelBaseType);
        ScoreBalance ResolveAchievementScoreBalance(string id);
    }

    [Serializable]
    public class ScoreBalance
    {
        public int Score { get; set; }
        public Balance Balance { get; set; }

        public bool IsEmpty()
        {
            return Score == 0 && (Balance == null || Balance.Values.All(pair => pair.Value == 0));
        }
    }
    public class ServerAchievement
    {
        public string Name { get; set; }
        public string AchievementId { get; set; }
        public int Score { get; set; }
        public Dictionary<string, int> Values { get; set; }
    }
}
