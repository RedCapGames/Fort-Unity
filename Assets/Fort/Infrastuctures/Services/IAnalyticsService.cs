using System;
using Fort.Info;
using Fort.Info.GameLevel;
using Fort.Info.Market.Iap;

namespace Fort
{
    /// <summary>
    /// This service is used to stat Fort and custom analytic item
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Stat item purchase
        /// </summary>
        /// <param name="itemId">The Id of Purchased item or Purchase item level</param>
        /// <param name="cost">The cost of purchase</param>
        /// <param name="discount">The discount or purchase</param>
        void StatItemPurchased(string itemId, Balance cost, int discount);
        /// <summary>
        /// Stat item rent
        /// </summary>
        /// <param name="itemId">The Id of Purchased item or Purchase item level</param>
        /// <param name="duration">Rent duration</param>
        void StatItemRent(string itemId, TimeSpan duration);
        /// <summary>
        /// Stat achievemt claim
        /// </summary>
        /// <param name="achievementId">The Id of Achievement or Achievement item level</param>
        /// <param name="award"></param>
        void StatAchievementClaimed(string achievementId, ScoreBalance award);
        /// <summary>
        /// Stat Iap package purchase
        /// </summary>
        /// <param name="iapPackage">Corresponding iap package</param>
        /// <param name="market">The name of the market that the purchase occurred in</param>
        void StatIapPurchased(IapPackageInfo iapPackage, string market);
        /// <summary>
        /// Stat Iap package purchase failed
        /// </summary>
        /// <param name="iapPackage">Corresponding iap package</param>
        /// <param name="purchaseToken">Market returned purchase token</param>
        /// <param name="market">The name of the market that the purchase occurred in</param>
        /// <param name="iapPurchaseFail">Failed reason. Possible values (Cancel,MarketFailed,FortServerFail,FraudDetected)</param>
        void StatIapFailed(IapPackageInfo iapPackage, string purchaseToken, string market, IapPurchaseFail iapPurchaseFail);
        /// <summary>
        /// Stat Iap package retry
        /// </summary>
        /// <param name="iapPackage">Corresponding iap package</param>
        /// <param name="purchaseToken">Market returned purchase token</param>
        /// <param name="market">The name of the market that the purchase occurred in</param>
        void StatIapRetry(IapPackageInfo iapPackage, string purchaseToken, string market);
        /// <summary>
        /// Stat Iap package retry failed
        /// </summary>
        /// <param name="iapPackage">Corresponding iap package</param>
        /// <param name="purchaseToken">Market returned purchase token</param>
        /// <param name="market">The name of the market that the purchase occurred in</param>
        /// <param name="iapRetryFail">Failed reason. Possible values (FortServerFail,FraudDetected)</param>
        void StatIapRetryFail(IapPackageInfo iapPackage, string purchaseToken, string market, IapRetryFail iapRetryFail);
        /// <summary>
        /// Stat video showing request
        /// </summary>
        /// <param name="advertismentProvider">The provider of advertisement</param>
        /// <param name="zone">The zone of advertisement</param>
        /// <param name="skipable">Is requested video skipable</param>
        void StatVideoRequest(string advertismentProvider, int zone, bool skipable);
        /// <summary>
        /// Stat video showing result
        /// </summary>
        /// <param name="advertismentProvider">The provider of advertisement</param>
        /// <param name="zone">The zone of advertisement</param>
        /// <param name="skipable">Is requested video skipable</param>
        /// <param name="videoResult">The result of showing video.Possible values (Succeeded,Cancel,NoVideoAvilable,ProviderError)</param>
        void StatVideoResult(string advertismentProvider, int zone, bool skipable, ShowVideoResult videoResult);
        /// <summary>
        /// Stat showing standard banner
        /// </summary>
        /// <param name="advertismentProvider">The provider of advertisement</param>
        void StatStandardBanner(string advertismentProvider);
        /// <summary>
        /// Stat showing interstitial banner 
        /// </summary>
        /// <param name="advertismentProvider">The provider of advertisement</param>
        void StatInterstitialBanner(string advertismentProvider);
        /// <summary>
        /// Stat sharing of invitation
        /// </summary>
        void StatInvitationShare();
        /// <summary>
        /// Stat applying of invitation
        /// </summary>
        void StatInvitationApplied();
        /// <summary>
        /// Stat user registeration
        /// </summary>
        void StatUserRegisterd();
        /// <summary>
        /// Stat the finishing of game level
        /// </summary>
        /// <param name="gameLevelInfo">The corresponding game level info</param>
        /// <param name="levelFinishStat">The stat of game level</param>
        void StatGameLevelFinished(GameLevelInfo gameLevelInfo, ILevelFinishStat levelFinishStat);
        /// <summary>
        /// Stat loading of scene
        /// </summary>
        /// <param name="sceneName">The name of scene</param>
        void StatSceneLoaded(string sceneName);
        /// <summary>
        /// Stat custom events
        /// </summary>
        /// <param name="name">Name of event</param>
        /// <param name="label">Label of event</param>
        /// <param name="category">category of event</param>
        /// <param name="value">value of event</param>
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
