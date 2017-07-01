using System;
using System.Collections.Generic;
using System.Linq;
using Fort.Events;
using Fort.Info;
using Fort.Info.Achievement;
using Fort.Inspector;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof (IAchievementService))]
    public class AchievementService : MonoBehaviour, IAchievementService
    {
        #region  Public Methods

        public void OnServerAchievementResolved(Dictionary<string,ServerAchievementInfo> achievementInfos,string[] claimedAchievementIds)
        {
            AchievementCache achievementCache =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AchievementCache>();
            if (achievementCache == null)
                achievementCache = new AchievementCache();
            foreach (string claimedAchievementId in claimedAchievementIds)
            {
                achievementCache.ServerAchievementIds[claimedAchievementId] = true;
            }
            ServiceLocator.Resolve<IStorageService>().UpdateData(achievementCache);

            AchievementStoredData achievementStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AchievementStoredData>();
            if (achievementStoredData == null)
                achievementStoredData = new AchievementStoredData();
            Dictionary<AchievementInfo, AchievementToken[]> achievementTokenses =
                claimedAchievementIds.Where(s => InfoResolver.Resolve<FortInfo>().Achievement.AchievementTokens.ContainsKey(s))
                    .Select(s => InfoResolver.Resolve<FortInfo>().Achievement.AchievementTokens[s])
                    .GroupBy(token => token.AchievementInfo)
                    .ToDictionary(tokens => tokens.Key, tokens => tokens.Select(token => token).ToArray());
            foreach (KeyValuePair<AchievementInfo, AchievementToken[]> pair in achievementTokenses)
            {
                if (pair.Value.Length > 0)
                {
                    if (pair.Key is NoneLevelBaseAchievementInfo)
                    {
                        achievementStoredData.Achievements[pair.Key.Id] = 0;
                    }
                    else
                    {
                        int max = pair.Value.Max(token => token.Index);
                        if (
                            !(achievementStoredData.Achievements.ContainsKey(pair.Key.Id) &&
                              achievementStoredData.Achievements[pair.Key.Id] >= max))
                        {
                            achievementStoredData.Achievements[pair.Key.Id] = max;
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, ServerAchievementInfo> pair in achievementInfos)
            {
                achievementStoredData.ServerAchievementInfos[pair.Key] = pair.Value;
            }
            ServiceLocator.Resolve<IStorageService>().UpdateData(achievementStoredData);
        }

        public string[] GetNotSyncAchievementIds()
        {
            AchievementCache achievementCache =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AchievementCache>() ?? new AchievementCache();
            return
                achievementCache.ServerAchievementIds.Where(pair => pair.Value == false)
                    .Select(pair => pair.Key)
                    .ToArray();
        }
        #endregion

        #region IAchievementService Members

        public void ClaimAchievement(Type noneLevelBaseType)
        {
            NoneLevelBaseAchievementInfo noneLevelBaseAchievementInfo =
                (NoneLevelBaseAchievementInfo) InfoResolver.Resolve<FortInfo>().Achievement.AchievementTypes[noneLevelBaseType];
            string achievementId = noneLevelBaseAchievementInfo.Id;
            AchievementStoredData achievementStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AchievementStoredData>();
            if (achievementStoredData == null)
                achievementStoredData = new AchievementStoredData();
            if (achievementStoredData.Achievements.ContainsKey(achievementId))
                return; //throw new Exception("Achievement Already Claimed");
            achievementStoredData.Achievements.Add(achievementId, 0);
            ServiceLocator.Resolve<IStorageService>().UpdateData(achievementStoredData);
            //Add to Server cashe
            AchievementCache achievementCache =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AchievementCache>();
            if (achievementCache == null)
                achievementCache = new AchievementCache();
            if (!achievementCache.ServerAchievementIds.ContainsKey(achievementId))
            {
                achievementCache.ServerAchievementIds.Add(achievementId, false);
            }
            ServiceLocator.Resolve<IStorageService>().UpdateData(achievementCache);
            ScoreBalance scoreBalance = ResolveAchievementScoreBalance(noneLevelBaseAchievementInfo.Id);
            ServiceLocator.Resolve<IUserManagementService>()
                .AddScoreAndBalance(scoreBalance.Score, scoreBalance.Balance);
            ServiceLocator.Resolve<IAnalyticsService>().StatAchievementClaimed(noneLevelBaseAchievementInfo.Id,scoreBalance);
            ServiceLocator.Resolve<IEventAggregatorService>().GetEvent<AchievementClaimedEvent>().Publish(new NoneLeveBaseAchievementClaimedEventArgs(noneLevelBaseAchievementInfo));
        }

        public void ClaimAchievement(Type levelBaseType, int achivementLevelIndex)
        {
            AchievementInfo achievementinfo = InfoResolver.Resolve<FortInfo>().Achievement.AchievementTypes[levelBaseType];
            Array achivementLevelInfos =
                (Array) achievementinfo.GetType().GetProperty("LevelInfo").GetValue(achievementinfo, new object[0]);
            if (achivementLevelIndex >= achivementLevelInfos.Length)
                throw new Exception("Claim Achievement AchievementLevelInfoes out of index");
            //AchivementLevelInfo achivementLevelInfo = (AchivementLevelInfo)achivementLevelInfos.GetValue(achivementLevelIndex);
            string achievementId = achievementinfo.Id;
            AchievementStoredData achievementStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AchievementStoredData>();
            if (achievementStoredData == null)
                achievementStoredData = new AchievementStoredData();
            if (achievementStoredData.Achievements.ContainsKey(achievementId) &&
                achievementStoredData.Achievements[achievementId] <= achivementLevelIndex)
                return; //throw new Exception("Achievement Already Claimed");
            achievementStoredData.Achievements[achievementId] = achivementLevelIndex;
            ServiceLocator.Resolve<IStorageService>().UpdateData(achievementStoredData);

            string[] possibleAchievementIds =
                achivementLevelInfos.Cast<AchievementLevelInfo>()
                    .Take(achivementLevelIndex + 1)
                    .Select(info => info.Id)
                    .ToArray();
            AchievementCache achievementCache =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AchievementCache>();
            if (achievementCache == null)
                achievementCache = new AchievementCache();
            foreach (string possibleAchievementId in possibleAchievementIds)
            {
                if (!achievementCache.ServerAchievementIds.ContainsKey(possibleAchievementId))
                {
                    achievementCache.ServerAchievementIds.Add(possibleAchievementId, false);
                }
            }
            ServiceLocator.Resolve<IStorageService>().UpdateData(achievementCache);
            AchievementLevelInfo achievementLevelInfo =
                (AchievementLevelInfo) achivementLevelInfos.GetValue(achivementLevelIndex);
            ScoreBalance scoreBalance = ResolveAchievementScoreBalance(achievementLevelInfo.Id);
            ServiceLocator.Resolve<IUserManagementService>()
                .AddScoreAndBalance(scoreBalance.Score, scoreBalance.Balance);
            ServiceLocator.Resolve<IAnalyticsService>().StatAchievementClaimed(achievementLevelInfo.Id,scoreBalance);
            ServiceLocator.Resolve<IEventAggregatorService>().GetEvent<AchievementClaimedEvent>().Publish(new LevelBaseAchievementClaimedEventArgs((LevelBaseAchievementInfo) achievementinfo,achivementLevelIndex));
        }

        public bool IsAchievementClaimed(Type noneLevelBaseType)
        {
            string achievementId = InfoResolver.Resolve<FortInfo>().Achievement.AchievementTypes[noneLevelBaseType].Id;
            AchievementStoredData achievementStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AchievementStoredData>();
            if (achievementStoredData == null)
                achievementStoredData = new AchievementStoredData();
            return achievementStoredData.Achievements.ContainsKey(achievementId);
        }

        public int GetAchievementClaimedIndex(Type levelBaseType)
        {
            AchievementInfo achievementinfo = InfoResolver.Resolve<FortInfo>().Achievement.AchievementTypes[levelBaseType];
            AchievementStoredData achievementStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AchievementStoredData>();
            if (achievementStoredData == null)
                achievementStoredData = new AchievementStoredData();
            if (achievementStoredData.Achievements.ContainsKey(achievementinfo.Id))
                return achievementStoredData.Achievements[achievementinfo.Id];
            return -1;
        }

        public ScoreBalance ResolveAchievementScoreBalance(string id)
        {
            AchievementStoredData achievementStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AchievementStoredData>();
            if (achievementStoredData == null)
                achievementStoredData = new AchievementStoredData();
            if (achievementStoredData.ServerAchievementInfos.ContainsKey(id))
            {
                return new ScoreBalance
                {
                    Balance = achievementStoredData.ServerAchievementInfos[id].Balance,
                    Score = achievementStoredData.ServerAchievementInfos[id].Score
                };
            }
            AchievementToken achievementToken = InfoResolver.Resolve<FortInfo>().Achievement.AchievementTokens[id];
            if (achievementToken.NoneLevelBase)
            {
                return new ScoreBalance
                {
                    Balance = ((NoneLevelBaseAchievementInfo) achievementToken.AchievementInfo).Balance,
                    Score = ((NoneLevelBaseAchievementInfo) achievementToken.AchievementInfo).Score
                };
            }
            return new ScoreBalance
            {
                Balance = achievementToken.AchievementLevelInfo.Balance,
                Score = achievementToken.AchievementLevelInfo.Score
            };
        }

        #endregion

        #region Nested types

        [Serializable]
        public class AchievementStoredData
        {
            #region Constructors

            public AchievementStoredData()
            {
                Achievements = new Dictionary<string, int>();
                ServerAchievementInfos = new Dictionary<string, ServerAchievementInfo>();
            }

            #endregion

            #region Properties

            public Dictionary<string, int> Achievements { get; set; }
            public Dictionary<string, ServerAchievementInfo> ServerAchievementInfos { get; set; }

            #endregion
        }
        [Serializable]
        public class ServerAchievementInfo
        {
            #region Properties

            public int Score { get; set; }
            public Balance Balance { get; set; }

            #endregion
        }

        [Serializable]
        public class AchievementCache
        {
            #region Constructors

            public AchievementCache()
            {
                ServerAchievementIds = new Dictionary<string, bool>();
            }

            #endregion

            #region Properties

            public Dictionary<string, bool> ServerAchievementIds { get; set; }

            #endregion
        }

        #endregion
    }
}