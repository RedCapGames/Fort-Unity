using Newtonsoft.Json;

namespace Fort.Info.Market.Iap
{
    public class ValueIapData
    {
        public ValueIapData()
        {
            Values = new Balance();
        }        
        public Balance Values { get; set; }
    }
}