using Fort.Analytics;

namespace Fort.Info
{
    public class Analytic
    {
        public Analytic()
        {
            StatItemPurchased = true;
            StatItemRented = true;
            StatIapPackePurchases = true;
            StatAchievementClaimed = true;
            StatVideo = true;
            StatStandardBanner = true;
            StatInterstitialBanner = true;
            StatInvitationShare = true;
            StatInvitationApplied = true;
            StatUserRegistered = true;
            StatGameLevelFinished = true;
            StatSceneLoad = true;
            StatCustomEvent = true;
        }

        public IAnalyticsProvider AnalyticsProvider { get; set; }
        public bool StatItemPurchased { get; set; }
        public bool StatItemRented { get; set; }
        public bool StatIapPackePurchases { get; set; }
        public bool StatAchievementClaimed { get; set; }
        public bool StatVideo { get; set; }
        public bool StatStandardBanner { get; set; }
        public bool StatInterstitialBanner { get; set; }
        public bool StatInvitationShare { get; set; }
        public bool StatInvitationApplied { get; set; }
        public bool StatUserRegistered { get; set; }
        public bool StatGameLevelFinished { get; set; }
        public bool StatSceneLoad { get; set; }
        public bool StatCustomEvent { get; set; }
    }
}
