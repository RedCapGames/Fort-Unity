using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort.Market
{
    public interface IMarket
    {
        Promise<string, MarketPurchaseError> PurchasePackage(string sku, string payload);
    }

    public enum MarketPurchaseError
    {
        Cancel,
        Failed
    }
}
