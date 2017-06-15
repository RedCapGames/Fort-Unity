using System;
using Fort.Info;

namespace Fort
{
    public interface IAnalyticsService
    {
        void StatItemPurchased(string itemId, Balance cost, int discount);
        void StatItemRent(string itemId, TimeSpan duration);
        void StatAchievementClaimed(string achievementId, ScoreBalance award);
        void StatIapPurchased(IapPackageInfo iapPackage, string market);
        void StatIapFailed(IapPackageInfo iapPackage, string purchaseToken, string market, IapPurchaseFail iapPurchaseFail);
        void StatIapRetry(IapPackageInfo iapPackage, string purchaseToken, string market);
        void StatIapRetryFail(IapPackageInfo iapPackage, string purchaseToken, string market, IapRetryFail iapRetryFail);
        void StatVideoRequest(string advertismentProvider, int zone, bool skipable);
        void StatVideoResult(string advertismentProvider, int zone, bool skipable, ShowVideoResult videoResult);
        void StatStandardBanner(string advertismentProvider);
        void StatInterstitialBanner(string advertismentProvider);
        void StatInvitationShare();
        void StatInvitationApplied();
        void StatUserRegisterd();
        void StatGameLevelFinished(GameLevelInfo gameLevelInfo, ILevelFinishStat levelFinishStat);
        void StatSceneLoaded(string sceneName);
        void StateCustomEvent(string name, string label, string category, object value);
    }

    public enum IapPurchaseFail
    {
        Cancel,
        MarketFailed,
        FortServerFail,
        FraudDetected
    }
    public enum IapRetryFail
    {
        FortServerFail,
        FraudDetected
    }

    public enum ShowVideoResult
    {
        Succeeded,
        Cancel,
        NoVideoAvilable,
        ProviderError
    }
}
