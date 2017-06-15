using Fort.Info;

namespace Fort.Analytics
{
    public interface IAnalyticsProvider
    {
        void StateEvent(string name, string label,string category, object value);
        void StatIapPackagePurchased(string sku, string label, int price,string market);
    }
}
