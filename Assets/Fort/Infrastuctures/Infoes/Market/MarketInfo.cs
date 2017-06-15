using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Inspector;

namespace Fort.Info
{
    public abstract class MarketInfo
    {
        [IgnoreProperty]
        public string MarketName { get; private set; }
        [IgnoreProperty]
        public string MarketDisplayName { get; private set; }
        public string ApplicationUrl { get; set; }

        protected MarketInfo(string marketName,string marketDisplayName)
        {
            MarketName = marketName;
            MarketDisplayName = marketDisplayName;
        }
    }
}
