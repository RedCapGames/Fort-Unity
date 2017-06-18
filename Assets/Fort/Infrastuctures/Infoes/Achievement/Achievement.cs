using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fort.Inspector;
using Newtonsoft.Json;

namespace Fort.Info.Achievement
{
    public class Achievement
    {
        #region Fields

        private AchievementInfo[] _achievementInfos;
        private Dictionary<string, AchievementToken> _achievementTokens;
        private Dictionary<Type, AchievementInfo> _achievementTypes;

        #endregion

        #region Properties

        [JsonIgnore]
        [IgnoreProperty]
        public Dictionary<string, AchievementToken> AchievementTokens
        {
            get
            {
                if (_achievementTokens == null)
                {
                    SyncAchievementTokens();
                }
                return _achievementTokens;
            }
            //set { _achievementTokens = value; }
        }

        [JsonIgnore]
        [IgnoreProperty]
        public Dictionary<Type, AchievementInfo> AchievementTypes
        {
            get
            {
                if (_achievementTypes == null)
                    SyncAchievementTypes();
                return _achievementTypes;
            }
            //set { _achievementTypes = value; }
        }

#if UNITY_EDITOR
        [Inspector(Presentation = "Fort.CustomEditor.AchievementsPresentation")]
#endif
        public AchievementInfo[] AchievementInfos
        {
            get
            {
                SyncAchievements();
                return _achievementInfos;
            }
            set
            {
                _achievementInfos = value;
                SyncAchievementTokens();
                SyncAchievementTypes();
            }
        }

        #endregion

        #region Private Methods

        private void SyncAchievements()
        {
            Type[] allAchievementTypes =
               TypeExtensions.GetAllTypes()
                    .Where(type => typeof (AchievementInfo).IsAssignableFrom(type))
                    .ToArray();
            allAchievementTypes =
                allAchievementTypes.ToArray()
                    .Where(
                        type =>
                            allAchievementTypes.Where(type1 => type1 != type)
                                .All(type1 => !type.IsAssignableFrom(type1)) && !type.IsAbstract)
                    .ToArray();
            AchievementInfo[] achievementInfos = allAchievementTypes.Select(type => Activator.CreateInstance(type))
                .Cast<AchievementInfo>()
                .ToArray();
            if (_achievementInfos == null)
                _achievementInfos = achievementInfos;
            else
            {
                _achievementInfos = _achievementInfos.Where(info => info != null).ToArray();
                _achievementInfos =
                    achievementInfos.Select(
                        info =>
                            _achievementInfos.FirstOrDefault(
                                achievementInfo => achievementInfo.GetType() == info.GetType()) ?? info).ToArray();
            }
        }

        private void SyncAchievementTokens()
        {
            _achievementTokens = new Dictionary<string, AchievementToken>();
            _achievementInfos = _achievementInfos ?? new AchievementInfo[0];
            foreach (AchievementInfo achievementInfo in _achievementInfos)
            {
                if (achievementInfo is NoneLevelBaseAchievementInfo)
                {
                    _achievementTokens.Add(achievementInfo.Id, new AchievementToken
                    {
                        AchievementInfo = achievementInfo,
                        NoneLevelBase = true
                    });
                }
                else
                {
                    PropertyInfo propertyInfo = achievementInfo.GetType().GetProperty("LevelInfoes");
                    int index = 0;
                    object value = propertyInfo.GetValue(achievementInfo, new object[0]);
                    if (value != null)
                    {
                        foreach (
                            AchievementLevelInfo achivementLevelInfo in
                                ((IEnumerable) value).Cast<AchievementLevelInfo>())
                        {
                            _achievementTokens.Add(achivementLevelInfo.Id, new AchievementToken
                            {
                                AchievementInfo = achievementInfo,
                                AchievementLevelInfo = achivementLevelInfo,
                                Index = index,
                                NoneLevelBase = false
                            });
                            index++;
                        }
                    }
                }
            }
        }

        private void SyncAchievementTypes()
        {
            _achievementInfos = _achievementInfos ?? new AchievementInfo[0];
            _achievementTypes = _achievementInfos.ToDictionary(info => info.GetType(), info => info);
        }

        #endregion
    }

    public class AchievementToken
    {
        #region Properties

        public bool NoneLevelBase { get; set; }
        public int Index { get; set; }
        public AchievementInfo AchievementInfo { get; set; }
        public AchievementLevelInfo AchievementLevelInfo { get; set; }

        #endregion
    }
}