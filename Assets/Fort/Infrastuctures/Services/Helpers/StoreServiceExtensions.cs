using System;
using System.Linq;
using Fort.Info;

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

        /*        public static bool IsNoneLevelBaseItemPurchased(this IStoreService storeService,NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo);
                public static int GetLevelBaseItemPurchasedIndex(LevelBasePurchasableItemInfo levelBasePurchasableItemInfo);*/
    }
}