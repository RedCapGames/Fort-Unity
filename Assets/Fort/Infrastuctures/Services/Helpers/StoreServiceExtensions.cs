using System;
using System.Linq;
using Fort.Info.Market.Iap;
using Fort.Info.PurchasableItem;

namespace Fort
{
    public static class StoreServiceExtensions
    {
        public static ComplitionPromise<IapPackageInfo[]> ResolvePackages<T>(this IStoreService storeService) where T : IapPackageInfo
        {
            ComplitionDeferred<IapPackageInfo[]> deferred = new ComplitionDeferred<IapPackageInfo[]>();
            storeService.ResolvePackages().Then(infos =>
            {

                deferred.Resolve(
                    infos.Where(
                        info =>
                            info is T ||
                            ((info is DiscountIapPackage) && ((DiscountIapPackage)info).PackageData.IapPackageInfo is T))
                        .ToArray());
            }, () => deferred.Reject());
            return deferred.Promise();
        }
        public static void SetDiscount<T>(this IStoreService storeService, int discount, TimeSpan duration)
            where T : IapPackageInfo
        {
            storeService.SetDiscount(typeof(T),discount,duration);
        }
        public static void SetDiscount<T>(this IStoreService storeService, int discount)
    where T : IapPackageInfo
        {
            storeService.SetDiscount(typeof(T), discount);
        }
        public static void SetDiscount(this IStoreService storeService, Type packageType, int discount)
        {
            storeService.SetDiscount(packageType, discount, TimeSpan.MaxValue);
        }
        public static int GetDiscount<T>(this IStoreService storeService)
        {
            return storeService.GetDiscount(typeof (T));
        }
        public static void RemoveDiscount<T>(this IStoreService storeService)
        {
            storeService.RemoveDiscount(typeof(T));
        }
        public static bool IsItemUsable(this NoneLevelBasePurchasableItemInfo purchasableItemInfo)
        {
            return ServiceLocator.Resolve<IStoreService>().IsItemUsable(purchasableItemInfo.Id);
        }
        public static bool IsItemUsable(this PurchasableLevelInfo purchasableLevelInfo)
        {
            return ServiceLocator.Resolve<IStoreService>().IsItemUsable(purchasableLevelInfo.Id);
        }
        public static bool IsItemPurchased(this NoneLevelBasePurchasableItemInfo purchasableItemInfo)
        {
            return ServiceLocator.Resolve<IStoreService>().IsItemPurchased(purchasableItemInfo.Id);
        }
        public static bool IsItemPurchased(this PurchasableLevelInfo purchasableLevelInfo)
        {
            return ServiceLocator.Resolve<IStoreService>().IsItemPurchased(purchasableLevelInfo.Id);
        }
        public static bool IsItemRented(this NoneLevelBasePurchasableItemInfo purchasableItemInfo)
        {
            return ServiceLocator.Resolve<IStoreService>().IsItemRented(purchasableItemInfo.Id);
        }
        public static bool IsItemRented(this PurchasableLevelInfo purchasableLevelInfo)
        {
            return ServiceLocator.Resolve<IStoreService>().IsItemRented(purchasableLevelInfo.Id);
        }
        public static void PurchaseItem(this IStoreService storeService, PurchasableItemInfo purchasableItem)
        {
            storeService.PurchaseItem(purchasableItem,0);
        }
        public static int GetLastPurchasedLevelIndex(this LevelBasePurchasableItemInfo purchasableItemInfo)
        {
            var lastItem = purchasableItemInfo.GetPurchasableLevelInfos()
                .Select((info, i) => new {Index = i, Info = info})
                .LastOrDefault(arg1 => arg1.Info.IsItemPurchased());
            if (lastItem == null)
                return -1;
            return lastItem.Index;
        }
        public static int GetLastUsableLevelIndex(this LevelBasePurchasableItemInfo purchasableItemInfo)
        {
            var lastItem = purchasableItemInfo.GetPurchasableLevelInfos()
                .Select((info, i) => new { Index = i, Info = info })
                .LastOrDefault(arg1 => arg1.Info.IsItemUsable());
            if (lastItem == null)
                return -1;
            return lastItem.Index;
        }
        public static T GetLastPurchasedLevel<T>(this LevelBasePurchasableItemInfo<T> purchasableItemInfo)
            where T : PurchasableLevelInfo
        {
            int lastPurchasedLevelIndex = GetLastPurchasedLevelIndex(purchasableItemInfo);
            if (lastPurchasedLevelIndex < 0)
                return null;
            return purchasableItemInfo.LevelInfoes[lastPurchasedLevelIndex];

        }
        public static T GetLastUsableLevel<T>(this LevelBasePurchasableItemInfo<T> purchasableItemInfo)
            where T : PurchasableLevelInfo
        {
            int lastUsableLevelIndex = GetLastUsableLevelIndex(purchasableItemInfo);
            if (lastUsableLevelIndex < 0)
                return null;
            return purchasableItemInfo.LevelInfoes[lastUsableLevelIndex];

        }

    }
}