using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Inspector;

namespace Fort.Info.Market
{
    public abstract class MarketInfo
    {
        [IgnorePresentation]
        public string MarketName { get; private set; }
        [IgnorePresentation]
        public string MarketDisplayName { get; private set; }
        public string ApplicationUrl { get; set; }

        public abstract Type MarketProvider { get; }

        protected MarketInfo(string marketName,string marketDisplayName)
        {
            MarketName = marketName;
            MarketDisplayName = marketDisplayName;
        }
    }
}
