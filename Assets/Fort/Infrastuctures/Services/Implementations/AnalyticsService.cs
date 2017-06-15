using System;
using Fort.Info;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(IAnalyticsService))]
    public class AnalyticsService : MonoBehaviour, IAnalyticsService
    {
        #region Implementation of IAnalyticsService

        public void StatItemPurchased(string itemId, Balance cost, int discount)
        {

            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;
            if (!InfoResolver.FortInfo.Analytic.StatItemPurchased)
                return;
            PurchasableToken purchasableToken = InfoResolver.FortInfo.Purchase.PurchasableTokens[itemId];
            if (purchasableToken.NoneLevelBase)
            {
                InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(purchasableToken.PurchasableItemInfo.Name,
                    purchasableToken.PurchasableItemInfo.DisplayName, "ItemPurchased",
                    new { ItemId = itemId, ItemName = purchasableToken.PurchasableItemInfo.Name, Cost = cost, Discount = discount });
            }
            else
            {
                InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(purchasableToken.PurchasableItemInfo.Name,
                    purchasableToken.PurchasableItemInfo.DisplayName, "ItemPurchased",
                    new
                    {
                        ItemId = itemId,
                        ItemName = purchasableToken.PurchasableItemInfo.Name,
                        Level = purchasableToken.Index,
                        Cost = cost,
                        Discount = discount
                    });
            }

        }

        public void StatItemRent(string itemId, TimeSpan duration)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;
            if (!InfoResolver.FortInfo.Analytic.StatItemRented)
                return;
            PurchasableToken purchasableToken = InfoResolver.FortInfo.Purchase.PurchasableTokens[itemId];
            if (purchasableToken.NoneLevelBase)
            {
                InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(purchasableToken.PurchasableItemInfo.Name,
                    purchasableToken.PurchasableItemInfo.DisplayName, "ItemRented",
                    new
                    {
                        ItemId = itemId,
                        ItemName = purchasableToken.PurchasableItemInfo.Name,

                        RentDuration = duration
                    });
            }
            else
            {
                InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(purchasableToken.PurchasableItemInfo.Name,
                    purchasableToken.PurchasableItemInfo.DisplayName, "ItemRented",
                    new
                    {
                        ItemId = itemId,
                        ItemName = purchasableToken.PurchasableItemInfo.Name,
                        Level = purchasableToken.Index,

                        RentDuration = duration
                    });
            }
        }
        public void StatAchievementClaimed(string achievementId, ScoreBalance award)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatAchievementClaimed)
                return;
            AchievementToken achievementToken = InfoResolver.FortInfo.Achievement.AchievementTokens[achievementId];
            if (achievementToken.NoneLevelBase)
            {
                InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(achievementToken.AchievementInfo.Name,
                    achievementToken.AchievementInfo.DisplayName, "AchievementClaimed",
                    new
                    {
                        AchievementId = achievementId,
                        AchievementName = achievementToken.AchievementInfo.Name,
                        Award = award
                    });
            }
            else
            {
                InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(achievementToken.AchievementInfo.Name,
                    achievementToken.AchievementInfo.DisplayName, "AchievementClaimed",
                    new
                    {
                        AchievementId = achievementId,
                        AchievementName = achievementToken.AchievementInfo.Name,
                        Index = achievementToken.Index,
                        Award = award
                    });
            }
        }

        public void StatIapPurchased(IapPackageInfo iapPackage, string market)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatIapPackePurchases)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StatIapPackagePurchased(iapPackage.Sku, iapPackage.DisplayName, iapPackage.Price, market);
        }

        public void StatIapFailed(IapPackageInfo iapPackage, string purchaseToken, string market, IapPurchaseFail iapPurchaseFail)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatIapPackePurchases)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(iapPackage.Sku,
                "Iap Failed", "IapFailed",
                new
                {
                    IapPackage = iapPackage,
                    PurchaseToken = purchaseToken,
                    Market = market,
                    FailType = iapPurchaseFail
                });
        }

        public void StatIapRetry(IapPackageInfo iapPackage, string purchaseToken, string market)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatIapPackePurchases)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(iapPackage.Sku,
                "Iap Retry", "IapRetry",
                new
                {
                    IapPackage = iapPackage,
                    PurchaseToken = purchaseToken,
                    Market = market
                });
        }

        public void StatIapRetryFail(IapPackageInfo iapPackage, string purchaseToken, string market, IapRetryFail iapRetryFail)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatIapPackePurchases)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(iapPackage.Sku,
                "Iap Retry Fail", "IapRetryFail",
                new
                {
                    IapPackage = iapPackage,
                    PurchaseToken = purchaseToken,
                    Market = market,
                    FailType = iapRetryFail
                });
        }

        public void StatVideoRequest(string advertismentProvider, int zone, bool skipable)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatVideo)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent("VideoRequest", "Video Request", "VideoRequest",
                new { AdvertismentProvider = advertismentProvider, Zone = zone, Skipable = skipable });
        }

        public void StatVideoResult(string advertismentProvider, int zone, bool skipable, ShowVideoResult videoResult)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatVideo)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent("VideoResult", "Video Result", "VideoResult",
                new { AdvertismentProvider = advertismentProvider, Zone = zone, Skipable = skipable, VideoResult = videoResult });
        }

        public void StatStandardBanner(string advertismentProvider)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatStandardBanner)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent("StandardBanner", "Standard Banner", "StandardBanner",
                new { AdvertismentProvider = advertismentProvider });
        }

        public void StatInterstitialBanner(string advertismentProvider)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;
            if (!InfoResolver.FortInfo.Analytic.StatInterstitialBanner)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent("InterstitialBanner", "Interstitial Banner", "InterstitialBanner",
                new { AdvertismentProvider = advertismentProvider });
        }

        public void StatInvitationShare()
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatInvitationShare)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent("InvitationShare", "Invitation Share", "InvitationShare", new { });
        }

        public void StatInvitationApplied()
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatInvitationApplied)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent("InvitationApplied", "Invitation Applied", "InvitationApplied", new { });
        }

        public void StatUserRegisterd()
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatUserRegistered)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent("UserRegistered", "User Registered", "UserRegistered", new { });
        }

        public void StatGameLevelFinished(GameLevelInfo gameLevelInfo, ILevelFinishStat levelFinishStat)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatGameLevelFinished)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(gameLevelInfo.Name, gameLevelInfo.DisplayName, "GameLevelFinished", new { LevelFinishStat = levelFinishStat, GameLevelId = gameLevelInfo.Id });
        }

        public void StatSceneLoaded(string sceneName)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;

            if (!InfoResolver.FortInfo.Analytic.StatSceneLoad)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent("SceneLoaded", "Scene Loaded", "SceneLoaded", new { SceneName = sceneName });
        }

        public void StateCustomEvent(string eventName, string label, string category, object value)
        {
            if (InfoResolver.FortInfo.Analytic.AnalyticsProvider == null)
                return;
            if (!InfoResolver.FortInfo.Analytic.StatCustomEvent)
                return;
            InfoResolver.FortInfo.Analytic.AnalyticsProvider.StateEvent(eventName, label, category, value);
        }

        #endregion
    }
}