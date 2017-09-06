using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fort;
using Fort.Analytics;
using Fort.Info;

namespace FortUnityAnalytics
{
    public class UnityAnalyticsProvider: IAnalyticsProvider
    {
        private readonly Dictionary<Type, Type> _numericTypes = new Dictionary<Type, Type>
        {
            {typeof (byte), typeof (byte)},
            {typeof (sbyte), typeof (sbyte)},
            {typeof (short), typeof (short)},
            {typeof (ushort), typeof (ushort)},
            {typeof (int), typeof (int)},
            {typeof (uint), typeof (uint)},
            {typeof (long), typeof (long)},
            {typeof (ulong), typeof (ulong)},
            {typeof (float), typeof (float)},
            {typeof (double), typeof (double)},
            {typeof (decimal), typeof (decimal)}
        };
        private IDictionary<string, object> ConvertAnalyticsStatValue(IAnalyticStatValue value)
        {
            ItemPurchaseAnalyticStat itemPurchaseAnalyticStat = value as ItemPurchaseAnalyticStat;
            if (itemPurchaseAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"ItemId", itemPurchaseAnalyticStat.ItemId},
                    {"ItemName", itemPurchaseAnalyticStat.ItemName},
                    {"Level", itemPurchaseAnalyticStat.Level},
                    {"Discount", itemPurchaseAnalyticStat.Discount},
                };
                foreach (string valueDefenition in FortInfo.Instance.ValueDefenitions)
                {
                    result.Add(string.Format("{0}Cost",valueDefenition),itemPurchaseAnalyticStat.Cost[valueDefenition]);
                }
                return result;
            }
            ItemRentAnalyticStat itemRentAnalyticStat = value as ItemRentAnalyticStat;
            if (itemRentAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"ItemId", itemRentAnalyticStat.ItemId},
                    {"ItemName", itemRentAnalyticStat.ItemName},
                    {"Level", itemRentAnalyticStat.Level},
                    {"Discount", itemRentAnalyticStat.Discount},
                    {"RentDuration", (float)itemRentAnalyticStat.RentDuration.TotalSeconds},
                };
                foreach (string valueDefenition in FortInfo.Instance.ValueDefenitions)
                {
                    result.Add(string.Format("{0}Cost", valueDefenition), itemRentAnalyticStat.Cost[valueDefenition]);
                }
                return result;

            }
            AchievementClaimedAnalyticStat achievementClaimedAnalyticStat = value as AchievementClaimedAnalyticStat;
            if (achievementClaimedAnalyticStat != null)
            {
                int score = 0;
                if (achievementClaimedAnalyticStat.Award != null)
                {
                    score = achievementClaimedAnalyticStat.Award.Score;
                }
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"AchievementId", achievementClaimedAnalyticStat.AchievementId},
                    {"AchievementName", achievementClaimedAnalyticStat.AchievementName},
                    {"Level", achievementClaimedAnalyticStat.Level},
                    {"AwardScore", score}
                };

                foreach (string valueDefenition in FortInfo.Instance.ValueDefenitions)
                {
                    if (achievementClaimedAnalyticStat.Award != null)
                        result.Add(string.Format("{0}Award", valueDefenition), achievementClaimedAnalyticStat.Award.Balance[valueDefenition]);
                }
                return result;
            }
            IapFailedAnalyticStat iapFailedAnalyticStat = value as IapFailedAnalyticStat;
            if (iapFailedAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"Sku", iapFailedAnalyticStat.IapPackage.Sku},
                    {"Market", iapFailedAnalyticStat.Market},
                    {"PurchaseToken", iapFailedAnalyticStat.PurchaseToken},
                    {"FailType", (int)iapFailedAnalyticStat.FailType}                    
                };
                return result;
            }
            IapRetryAnalyticStat iapRetryAnalyticStat = value as IapRetryAnalyticStat;
            if (iapRetryAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"Sku", iapRetryAnalyticStat.IapPackage.Sku},
                    {"Market", iapRetryAnalyticStat.Market},
                    {"PurchaseToken", iapRetryAnalyticStat.PurchaseToken}                    
                };
                return result;

            }
            IapRetryFailedAnalyticStat iapRetryFailedAnalyticStat = value as IapRetryFailedAnalyticStat;
            if (iapRetryFailedAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"Sku", iapRetryFailedAnalyticStat.IapPackage.Sku},
                    {"Market", iapRetryFailedAnalyticStat.Market},
                    {"PurchaseToken", iapRetryFailedAnalyticStat.PurchaseToken},
                    {"FailType", (int)iapRetryFailedAnalyticStat.FailType}
                };
                return result;
            }
            VideoRequestAnalyticStat videoRequestAnalyticStat = value as VideoRequestAnalyticStat;
            if (videoRequestAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"AdvertismentProvider", videoRequestAnalyticStat.AdvertismentProvider},
                    {"Zone", videoRequestAnalyticStat.Zone},
                    {"Skipable", videoRequestAnalyticStat.Skipable}
                };
                return result;
            }
            VideoResultAnalyticStat videoResultAnalyticStat = value as VideoResultAnalyticStat;
            if (videoResultAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"AdvertismentProvider", videoResultAnalyticStat.AdvertismentProvider},
                    {"Zone", videoResultAnalyticStat.Zone},
                    {"Skipable", videoResultAnalyticStat.Skipable},
                    {"VideoResult", (int)videoResultAnalyticStat.VideoResult}
                };
                return result;
            }
            StandardBannerAnalyticStat standardBannerAnalyticStat = value as StandardBannerAnalyticStat;
            if (standardBannerAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"AdvertismentProvider", standardBannerAnalyticStat.AdvertismentProvider}
                };
                return result;
            }
            InterstitialBannerAnalyticStat interstitialBannerAnalyticStat = value as InterstitialBannerAnalyticStat;
            if (interstitialBannerAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"AdvertismentProvider", interstitialBannerAnalyticStat.AdvertismentProvider}
                };
                return result;
            }
            GameLevelFinishedAnalyticStat gameLevelFinishedAnalyticStat = value as GameLevelFinishedAnalyticStat;
            if (gameLevelFinishedAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"CategoryId", gameLevelFinishedAnalyticStat.GameLevelCategoryId},
                    {"CategoryName", gameLevelFinishedAnalyticStat.GameLevelCategoryName},
                    {"LevelId", gameLevelFinishedAnalyticStat.GameLevelId},
                    {"LevelName", gameLevelFinishedAnalyticStat.GameLevelName}
                };
                if (gameLevelFinishedAnalyticStat.LevelFinishStat != null)
                {
                    if (gameLevelFinishedAnalyticStat.LevelFinishStat != null)
                    {
                        Type type = gameLevelFinishedAnalyticStat.LevelFinishStat.GetType();
                        PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        foreach (PropertyInfo propertyInfo in propertyInfos)
                        {
                            if (propertyInfo.PropertyType == typeof (string) ||
                                propertyInfo.PropertyType == typeof (bool) ||
                                _numericTypes.ContainsKey(propertyInfo.PropertyType))
                            {
                                if (!result.ContainsKey(propertyInfo.Name))
                                    result.Add(propertyInfo.Name,
                                        propertyInfo.GetValue(gameLevelFinishedAnalyticStat.LevelFinishStat,
                                            new object[0]));
                            }
                        }
                    }
                }
                return result;
            }
            SceneLoadedAnalyticStat sceneLoadedAnalyticStat = value as SceneLoadedAnalyticStat;
            if (sceneLoadedAnalyticStat != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>
                {
                    {"SceneName", sceneLoadedAnalyticStat.SceneName}
                };
                return result;
            }
            return new Dictionary<string, object>();
        }

        #region Implementation of IAnalyticsProvider

        public void Initialize()
        {
        }

        public void StateEvent(string name, string label, string category, IAnalyticStatValue value)
        {
            UnityEngine.Analytics.Analytics.CustomEvent(category, ConvertAnalyticsStatValue(value));
        }

        public void StatIapPackagePurchased(string sku, string label, int price, string market)
        {
            UnityEngine.Analytics.Analytics.Transaction(sku, price, "Rial");
        }

        #endregion
    }
}
