using System;
using System.Collections.Generic;
using Fort.Info;
using Fort.Info.Market.Iap;
using Fort.Info.PurchasableItem;

namespace Fort
{
    public interface IStoreService
    {
        void RentItem(NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo, TimeSpan rentDuration);
        void RentItem(PurchasableLevelInfo purchasableLevelInfo, TimeSpan rentDuration);
        void PurchaseItem(PurchasableItemInfo purchasableItem,int discount);
        bool IsItemUsable(string id);
        Balance ResolvePurchasableItemCost(string id);
        void SetDiscount(Type packageType, int discount,TimeSpan duration);
        int GetDiscount(Type packageType);
        void RemoveDiscount(Type packageType);
        ErrorPromise<PurchasePackageErrorResult> PurchasePackage(IapPackageInfo iapPackage);
        ErrorPromise<PurchasePackageErrorResult> ReportPurchasePackage(IapPackageInfo iapPackage, string purchaseToken);
        ComplitionPromise<IapPackageInfo[]> ResolvePackages();
    }


    public enum PurchasePackageErrorResult
    {
        Failed,
        Canceled,
        AnotherPurchaseInAction,
        MarketFailed
    }

    public class ServerPurchasableItem
    {
        public string Name { get; set; }
        public string ItemId { get; set; }
        public Dictionary<string, int> Costs { get; set; }
    }
}
