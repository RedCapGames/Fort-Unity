using System;
using System.Collections.Generic;
using System.Linq;
using Fort.Info;

namespace Fort
{
    /// <summary>
    /// This service is used to manage Achievement actions like claiming achievement and checking if achievement is claimed
    /// </summary>
    public interface IAchievementService
    {
        /// <summary>
        /// Claiming none level base achievement
        /// </summary>
        /// <param name="noneLevelBaseType">The type of none level base achievement</param>
        void ClaimAchievement(Type noneLevelBaseType);
        /// <summary>
        /// Claiming level base achievement
        /// </summary>
        /// <param name="levelBaseType">The type of level base achievement</param>
        /// <param name="achivementLevelIndex">Level of achievement that must be claimed</param>
        void ClaimAchievement(Type levelBaseType, int achivementLevelIndex);
        /// <summary>
        /// Is none level base achievement claimed
        /// </summary>
        /// <param name="noneLevelBaseType">The type of none level base achievement</param>
        /// <returns></returns>
        bool IsAchievementClaimed(Type noneLevelBaseType);
        /// <summary>
        /// Return the index of claimed level base achievement -1 if not claimed yet
        /// </summary>
        /// <param name="levelBaseType">The type of level base achievement</param>
        /// <returns>index of claimed level base achievement</returns>
        int GetAchievementClaimedIndex(Type levelBaseType);
        /// <summary>
        /// Resolving the score and balance of an achievement or achievement level by Id
        /// </summary>
        /// <param name="id">Id of achievement or achievement level</param>
        /// <returns></returns>
        ScoreBalance ResolveAchievementScoreBalance(string id);
    }

    /// <summary>
    /// This class encapsulates score and balance
    /// </summary>
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
