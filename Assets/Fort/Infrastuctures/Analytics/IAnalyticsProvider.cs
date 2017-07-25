using Fort.Info;

namespace Fort.Analytics
{
    public interface IAnalyticsProvider
    {
        void Initialize();
        void StateEvent(string name, string label,string category, IAnalyticStatValue value);
        void StatIapPackagePurchased(string sku, string label, int price,string market);
    }

    public interface IAnalyticStatValue
    {
        
    }
}
