using System;
using Fort.Info.Market;

namespace FortBazaar.Info
{
    public class BazaarMarketInfo:MarketInfo
    {
        public string Key { get; set; }
        public BazaarMarketInfo() 
            : base("Bazaar", "کافه بازار")
        {
        }

        #region Overrides of MarketInfo

        public override Type MarketProvider
        {
            get { return typeof (BazaarMarketProvider); }
        }

        #endregion
    }
}