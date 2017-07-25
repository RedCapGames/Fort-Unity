using System;
using Fort.Analytics;
using Fort.Info;
using Fort.Info.Achievement;
using Fort.Info.GameLevel;
using Fort.Info.Market.Iap;
using Fort.Info.PurchasableItem;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(IAnalyticsService),LoadOnInitialize = true)]
    public class AnalyticsService : MonoBehaviour, IAnalyticsService
    {
        void Start()
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.Initialize();
        }
        #region Implementation of IAnalyticsService

        public void StatItemPurchased(string itemId, Balance cost, int discount)
        {

            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;
            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatItemPurchased)
                return;
            PurchasableToken purchasableToken = InfoResolver.Resolve<FortInfo>().Purchase.PurchasableTokens[itemId];
            if (purchasableToken.NoneLevelBase)
            {
                InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent(purchasableToken.PurchasableItemInfo.Name,
                    purchasableToken.PurchasableItemInfo.DisplayName, "ItemPurchased",
                    new ItemPurchaseAnalyticStat { ItemId = itemId, ItemName = purchasableToken.PurchasableItemInfo.Name, Cost = cost, Discount = discount });
            }
            else
            {
                InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent(purchasableToken.PurchasableItemInfo.Name,
                    purchasableToken.PurchasableItemInfo.DisplayName, "ItemPurchased",
                    new ItemPurchaseAnalyticStat
                    {
                        ItemId = itemId,
                        ItemName = purchasableToken.PurchasableItemInfo.Name,
                        Level = purchasableToken.Index,
                        Cost = cost,
                        Discount = discount
                    });
            }

        }

        public void StatItemRent(string itemId, Balance cost, int discount, TimeSpan duration)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;
            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatItemRented)
                return;
            PurchasableToken purchasableToken = InfoResolver.Resolve<FortInfo>().Purchase.PurchasableTokens[itemId];
            if (purchasableToken.NoneLevelBase)
            {
                InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent(purchasableToken.PurchasableItemInfo.Name,
                    purchasableToken.PurchasableItemInfo.DisplayName, "ItemRented",
                    new ItemRentAnalyticStat
                    {
                        ItemId = itemId,
                        ItemName = purchasableToken.PurchasableItemInfo.Name,
                        Discount = discount,
                        Cost = cost,
                        RentDuration = duration
                    });
            }
            else
            {
                InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent(purchasableToken.PurchasableItemInfo.Name,
                    purchasableToken.PurchasableItemInfo.DisplayName, "ItemRented",
                    new ItemRentAnalyticStat
                    {
                        ItemId = itemId,
                        ItemName = purchasableToken.PurchasableItemInfo.Name,
                        Level = purchasableToken.Index,
                        Discount = discount,
                        Cost = cost,
                        RentDuration = duration
                    });
            }
        }
        public void StatAchievementClaimed(string achievementId, ScoreBalance award)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatAchievementClaimed)
                return;
            AchievementToken achievementToken = InfoResolver.Resolve<FortInfo>().Achievement.AchievementTokens[achievementId];
            if (achievementToken.NoneLevelBase)
            {
                InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent(achievementToken.AchievementInfo.Name,
                    achievementToken.AchievementInfo.DisplayName, "AchievementClaimed",
                    new AchievementClaimedAnalyticStat
                    {
                        AchievementId = achievementId,
                        AchievementName = achievementToken.AchievementInfo.Name,
                        Award = award
                    });
            }
            else
            {
                InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent(achievementToken.AchievementInfo.Name,
                    achievementToken.AchievementInfo.DisplayName, "AchievementClaimed",
                    new AchievementClaimedAnalyticStat
                    {
                        AchievementId = achievementId,
                        AchievementName = achievementToken.AchievementInfo.Name,
                        Level = achievementToken.Index,
                        Award = award
                    });
            }
        }

        public void StatIapPurchased(IapPackageInfo iapPackage, string market)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatIapPackePurchases)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StatIapPackagePurchased(iapPackage.Sku, iapPackage.DisplayName, iapPackage.Price, market);
        }

        public void StatIapFailed(IapPackageInfo iapPackage, string purchaseToken, string market, IapPurchaseFail iapPurchaseFail)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatIapPackePurchases)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent(iapPackage.Sku,
                "Iap Failed", "IapFailed",
                new IapFailedAnalyticStat
                {
                    IapPackage = iapPackage,
                    PurchaseToken = purchaseToken,
                    Market = market,
                    FailType = iapPurchaseFail
                });
        }

        public void StatIapRetry(IapPackageInfo iapPackage, string purchaseToken, string market)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatIapPackePurchases)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent(iapPackage.Sku,
                "Iap Retry", "IapRetry",
                new IapRetryAnalyticStat
                {
                    IapPackage = iapPackage,
                    PurchaseToken = purchaseToken,
                    Market = market
                });
        }

        public void StatIapRetryFail(IapPackageInfo iapPackage, string purchaseToken, string market, IapRetryFail iapRetryFail)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatIapPackePurchases)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent(iapPackage.Sku,
                "Iap Retry Fail", "IapRetryFail",
                new IapRetryFailedAnalyticStat
                {
                    IapPackage = iapPackage,
                    PurchaseToken = purchaseToken,
                    Market = market,
                    FailType = iapRetryFail
                });
        }

        public void StatVideoRequest(string advertismentProvider, string zone, bool skipable)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatVideo)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent("VideoRequest", "Video Request", "VideoRequest",
                new VideoRequestAnalyticStat { AdvertismentProvider = advertismentProvider, Zone = zone, Skipable = skipable });
        }

        public void StatVideoResult(string advertismentProvider, string zone, bool skipable, ShowVideoResult videoResult)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatVideo)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent("VideoResult", "Video Result", "VideoResult",
                new VideoResultAnalyticStat { AdvertismentProvider = advertismentProvider, Zone = zone, Skipable = skipable, VideoResult = videoResult });
        }

        public void StatStandardBanner(string advertismentProvider)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatStandardBanner)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent("StandardBanner", "Standard Banner", "StandardBanner",
                new StandardBannerAnalyticStat { AdvertismentProvider = advertismentProvider });
        }

        public void StatInterstitialBanner(string advertismentProvider)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;
            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatInterstitialBanner)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent("InterstitialBanner", "Interstitial Banner", "InterstitialBanner",
                new InterstitialBannerAnalyticStat { AdvertismentProvider = advertismentProvider });
        }

        public void StatInvitationShare()
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatInvitationShare)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent("InvitationShare", "Invitation Share", "InvitationShare", null);
        }

        public void StatInvitationApplied()
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatInvitationApplied)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent("InvitationApplied", "Invitation Applied", "InvitationApplied", null);
        }

        public void StatUserRegisterd()
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatUserRegistered)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent("UserRegistered", "User Registered", "UserRegistered", null);
        }

        public void StatGameLevelFinished(GameLevelInfo gameLevelInfo, ILevelFinishStat levelFinishStat)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatGameLevelFinished)
                return;
            GameLevelCategory gameLevelCategory =
                InfoResolver.Resolve<FortInfo>().GameLevel.LevelCategoriesParentMap[gameLevelInfo.Id];
            InfoResolver.Resolve<FortInfo>()
                .Analytic.AnalyticsProvider.StateEvent(gameLevelInfo.Name, gameLevelInfo.DisplayName,
                    "GameLevelFinished",
                    new GameLevelFinishedAnalyticStat
                    {
                        LevelFinishStat = levelFinishStat,
                        GameLevelId = gameLevelInfo.Id,
                        GameLevelName=gameLevelInfo.Name,
                        GameLevelCategoryId = gameLevelCategory.Id,
                        GameLevelCategoryName = gameLevelCategory.Name
                    });
        }

        public void StatSceneLoaded(string sceneName)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatSceneLoad)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent("SceneLoaded", "Scene Loaded", "SceneLoaded", new SceneLoadedAnalyticStat { SceneName = sceneName });
        }

        public void StateCustomEvent(string eventName, string label, string category, IAnalyticStatValue value)
        {
            if (InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider == null)
                return;
            if (!InfoResolver.Resolve<FortInfo>().Analytic.StatCustomEvent)
                return;
            InfoResolver.Resolve<FortInfo>().Analytic.AnalyticsProvider.StateEvent(eventName, label, category, value);
        }

        #endregion
    }

    public class ItemPurchaseAnalyticStat : IAnalyticStatValue
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public int Level { get; set; }
        public Balance Cost { get; set; }
        public int Discount { get; set; }
    }
    public class ItemRentAnalyticStat : IAnalyticStatValue
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public int Level { get; set; }
        public Balance Cost { get; set; }
        public int Discount { get; set; }
        public TimeSpan RentDuration { get; set; }
    }
    public class AchievementClaimedAnalyticStat : IAnalyticStatValue
    {
        public string AchievementId { get; set; }
        public string AchievementName { get; set; }
        public int Level { get; set; }
        public ScoreBalance Award { get; set; }
    }
    public class IapFailedAnalyticStat : IAnalyticStatValue
    {
        public IapPackageInfo IapPackage { get; set; }
        public string PurchaseToken { get; set; }
        public string Market { get; set; }
        public IapPurchaseFail FailType { get; set; }
    }
    public class IapRetryAnalyticStat : IAnalyticStatValue
    {
        public IapPackageInfo IapPackage { get; set; }
        public string PurchaseToken { get; set; }
        public string Market { get; set; }
    }
    public class IapRetryFailedAnalyticStat : IAnalyticStatValue
    {
        public IapPackageInfo IapPackage { get; set; }
        public string PurchaseToken { get; set; }
        public string Market { get; set; }
        public IapRetryFail FailType { get; set; }
    }

    public class VideoRequestAnalyticStat : IAnalyticStatValue
    {
        public string AdvertismentProvider { get; set; }
        public string Zone { get; set; }
        public bool Skipable { get; set; }
    }
    public class VideoResultAnalyticStat : IAnalyticStatValue
    {
        public string AdvertismentProvider { get; set; }
        public string Zone { get; set; }
        public bool Skipable { get; set; }
        public ShowVideoResult VideoResult { get; set; }
    }
    public class StandardBannerAnalyticStat : IAnalyticStatValue
    {
        public string AdvertismentProvider { get; set; }
    }
    public class InterstitialBannerAnalyticStat : IAnalyticStatValue
    {
        public string AdvertismentProvider { get; set; }
    }
    public class GameLevelFinishedAnalyticStat : IAnalyticStatValue
    {
        public string GameLevelId { get; set; }
        public string GameLevelName { get; set; }
        public string GameLevelCategoryId { get; set; }
        public string GameLevelCategoryName { get; set; }
        public ILevelFinishStat LevelFinishStat { get; set; }
    }
    public class SceneLoadedAnalyticStat : IAnalyticStatValue
    {
        public string SceneName { get; set; }
    }
}