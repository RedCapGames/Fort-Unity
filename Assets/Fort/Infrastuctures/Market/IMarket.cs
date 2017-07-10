using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort.Market
{
    public interface IMarketProvider
    {
        Promise<string, MarketPurchaseError> PurchasePackage(string sku, string payload);
    }

    public enum MarketPurchaseError
    {
        Cancel,
        Failed
    }
}
