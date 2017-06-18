using System;
using System.Collections.Generic;
using Fort.Info;
using Fort.Info.Market.Iap;
using Fort.Info.PurchasableItem;

namespace Fort
{
    public interface IStoreService
    {
        void RentItem(NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo,int discount, TimeSpan rentDuration);
        void RentItem(PurchasableLevelInfo purchasableLevelInfo, int discount, TimeSpan rentDuration);
        void PurchaseItem(PurchasableItemInfo purchasableItem,int discount);
        bool IsItemUsable(string id);
        ItemCosts ResolvePurchasableItemCost(string id);
        bool IsItemPurchased(string id);
        bool IsItemRented(string id);
        bool IsEnoughFundToPurchaseItem(PurchasableItemInfo purchasableItem, int discount);
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
        public Dictionary<string, int> RentCost { get; set; }
        public Dictionary<string, int> PurchaseCost { get; set; }
    }
}
