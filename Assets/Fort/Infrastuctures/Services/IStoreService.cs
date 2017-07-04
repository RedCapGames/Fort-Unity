using System;
using System.Collections.Generic;
using Fort.Info;
using Fort.Info.Market.Iap;
using Fort.Info.PurchasableItem;

namespace Fort
{
    public interface IStoreService
    {
        /// <summary>
        /// Rent none level base purchasable item
        /// </summary>
        /// <param name="noneLevelBasePurchasableItemInfo">Item to rent</param>
        /// <param name="discount">Discout for rent.(value betwean 0 and 100)</param>
        /// <param name="rentDuration">The duration of rent.</param>
        void RentItem(NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo,int discount, TimeSpan rentDuration);
        /// <summary>
        /// Rent Level base purchasable item
        /// </summary>
        /// <param name="purchasableLevelInfo">Level of item</param>
        /// <param name="discount">Discout for rent.(value betwean 0 and 100)</param>
        /// <param name="rentDuration">The duration of rent.</param>
        void RentItem(PurchasableLevelInfo purchasableLevelInfo, int discount, TimeSpan rentDuration);
        /// <summary>
        /// Purchase item.In Level base purchasable item first posibble Level will be purchased
        /// </summary>
        /// <param name="purchasableItem">Item to purchase</param>
        /// <param name="discount">Discout for purchase.(value betwean 0 and 100)</param>
        void PurchaseItem(PurchasableItemInfo purchasableItem,int discount);
        /// <summary>
        /// Return if item can be use in game.(purchased or rented)
        /// </summary>
        /// <param name="id">Id of purchase item or id of Level of purchase item</param>
        /// <returns>Item can be use in game.(purchased or rented)</returns>
        bool IsItemUsable(string id);
        /// <summary>
        /// Return if item purchase and rent cost
        /// </summary>
        /// <param name="id">Id of purchase item or id of Level of purchase item</param>
        /// <returns>Item purchase and rent cost</returns>
        ItemCosts ResolvePurchasableItemCost(string id);
        /// <summary>
        /// Return if item purchased
        /// </summary>
        /// <param name="id">Id of purchase item or id of Level of purchase item</param>
        /// <returns>Is item purchased</returns>
        bool IsItemPurchased(string id);
        /// <summary>
        /// Return if item Rented
        /// </summary>
        /// <param name="id">Id of purchase item or id of Level of purchase item</param>
        /// <returns>Is item rented</returns>
        bool IsItemRented(string id);
        /// <summary>
        /// Return if user has enough fund to purchase this item
        /// </summary>
        /// <param name="purchasableItem">Item to purchase</param>
        /// <param name="discount">Discout for purchase.(value betwean 0 and 100)</param>
        /// <returns>User has enough fund to purchase this item</returns>
        bool IsEnoughFundToPurchaseItem(PurchasableItemInfo purchasableItem, int discount);
        /// <summary>
        /// Set discount for Iap package
        /// </summary>
        /// <param name="packageType">Type of IapPackageInfo</param>
        /// <param name="discount">Discout for Iap package.(value betwean 0 and 100)</param>
        /// <param name="duration">Duration of discount</param>
        void SetDiscount(Type packageType, int discount,TimeSpan duration);
        /// <summary>
        /// Get discount of Iap package
        /// </summary>
        /// <param name="packageType">Type of IapPackageInfo</param>
        /// <returns>Discount</returns>
        int GetDiscount(Type packageType);
        /// <summary>
        /// Removing discount of Iap package
        /// </summary>
        /// <param name="packageType"></param>
        void RemoveDiscount(Type packageType);
        /// <summary>
        /// Purchase Iap package
        /// </summary>
        /// <param name="iapPackage">Corresponding Iap package</param>
        /// <returns>Promise of purchasing Iap package</returns>
        ErrorPromise<PurchasePackageErrorResult> PurchasePackage(IapPackageInfo iapPackage);
        /// <summary>
        /// In some cases the purchasing of Iap package Encounter a problem.Report Iap package will retry purchase Iap package.
        /// </summary>
        /// <param name="iapPackage">Corresponding Iap package</param>
        /// <param name="purchaseToken">Market purchase token</param>
        /// <returns>Promise of purchasing Iap package</returns>
        ErrorPromise<PurchasePackageErrorResult> ReportPurchasePackage(IapPackageInfo iapPackage, string purchaseToken);
        /// <summary>
        /// Resolving Iap package list. If server provider added this list will be loaded from server and prices will be synced from server.
        /// </summary>
        /// <returns>Promise of Iap package list.</returns>
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
