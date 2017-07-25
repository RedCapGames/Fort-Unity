using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fort.Events;
using Fort.Info;
using Fort.Info.Language;
using Fort.Info.Market;
using Fort.Info.Market.Iap;
using Fort.Info.PurchasableItem;
using Fort.Market;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof (IStoreService))]
    public class StoreService : MonoBehaviour, IStoreService
    {
        #region Fields


        private bool _isPurchasingPackage;
        private IMarketProvider _marketProvider;

        #endregion

        #region  Public Methods

        public void OnServerPurchaseResolved(Dictionary<string, ItemCosts> serverPurchasableBalance,
            string[] purchasedItemIds)
        {
            PurchasableItemCache purchasableItemCache =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemCache>() ??
                new PurchasableItemCache();
            foreach (string purchasedItemId in purchasedItemIds)
            {
                purchasableItemCache.ServerPurchasableitemIds[purchasedItemId] = true;
            }
            ServiceLocator.Resolve<IStorageService>().UpdateData(purchasableItemCache);

            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
            Dictionary<PurchasableItemInfo, PurchasableToken[]> purchasableTokenses =
                purchasedItemIds.Where(s => InfoResolver.Resolve<FortInfo>().Purchase.PurchasableTokens.ContainsKey(s))
                    .Select(s => InfoResolver.Resolve<FortInfo>().Purchase.PurchasableTokens[s])
                    .GroupBy(token => token.PurchasableItemInfo)
                    .ToDictionary(tokens => tokens.Key, tokens => tokens.Select(token => token).ToArray());
            foreach (KeyValuePair<PurchasableItemInfo, PurchasableToken[]> pair in purchasableTokenses)
            {
                if (pair.Value.Length > 0)
                {
                    if (pair.Key is NoneLevelBasePurchasableItemInfo)
                        purchasableItemStoredData.PurchasableItems[pair.Key.Id] = 0;
                    else
                    {
                        int max = pair.Value.Max(token => token.Index);
                        if (!(purchasableItemStoredData.PurchasableItems.ContainsKey(pair.Key.Id) &&
                              purchasableItemStoredData.PurchasableItems[pair.Key.Id] >= max))
                        {
                            purchasableItemStoredData.PurchasableItems[pair.Key.Id] = max;
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, ItemCosts> pair in serverPurchasableBalance)
            {
                purchasableItemStoredData.ServerPurchasableItemInfos[pair.Key] = pair.Value;
            }
            ServiceLocator.Resolve<IStorageService>().UpdateData(purchasableItemStoredData);
        }

        public string[] GetNotPurchasableIds()
        {
            PurchasableItemCache purchasableItemCache =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemCache>() ??
                new PurchasableItemCache();
            return
                purchasableItemCache.ServerPurchasableitemIds.Where(pair => !pair.Value)
                    .Select(pair => pair.Key)
                    .ToArray();
        }

        #endregion

        #region IStoreService Members

        public void RentItem(NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo, int discount, TimeSpan rentDuration)
        {
            if(noneLevelBasePurchasableItemInfo is ValuePurchasableItemInfo)
                throw new Exception("Value purchasable item cannot be rented");
            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
            DateTime rentTime = DateTime.Now + rentDuration;
            if (purchasableItemStoredData.Rents.ContainsKey(noneLevelBasePurchasableItemInfo.Id) &&
                purchasableItemStoredData.Rents[noneLevelBasePurchasableItemInfo.Id] > rentTime)
                return;
            Balance balance = ServiceLocator.Resolve<IUserManagementService>().Balance;
            Balance cost = ResolvePurchasableItemCost(noneLevelBasePurchasableItemInfo.Id).Rent * discount / 100f;
            if (cost > balance)
                throw new Exception("Insufficient funds");

            ServiceLocator.Resolve<IUserManagementService>().AddScoreAndBalance(0, -cost);

            purchasableItemStoredData.Rents[noneLevelBasePurchasableItemInfo.Id] = rentTime;
            ServiceLocator.Resolve<IStorageService>().UpdateData(purchasableItemStoredData);
            ServiceLocator.Resolve<IAnalyticsService>().StatItemRent(noneLevelBasePurchasableItemInfo.Id,cost,discount, rentDuration);
        }

        public void RentItem(PurchasableLevelInfo purchasableLevelInfo, int discount, TimeSpan rentDuration)
        {
            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
            DateTime rentTime = DateTime.Now + rentDuration;
            if (purchasableItemStoredData.Rents.ContainsKey(purchasableLevelInfo.Id) &&
                purchasableItemStoredData.Rents[purchasableLevelInfo.Id] > rentTime)
                return;
            purchasableItemStoredData.Rents[purchasableLevelInfo.Id] = rentTime;
            Balance balance = ServiceLocator.Resolve<IUserManagementService>().Balance;
            Balance cost = ResolvePurchasableItemCost(purchasableLevelInfo.Id).Rent * discount / 100f;
            if (cost > balance)
                throw new Exception("Insufficient funds");
            ServiceLocator.Resolve<IUserManagementService>().AddScoreAndBalance(0, -cost);

            ServiceLocator.Resolve<IStorageService>().UpdateData(purchasableItemStoredData);
            ServiceLocator.Resolve<IAnalyticsService>().StatItemRent(purchasableLevelInfo.Id,cost,discount, rentDuration);
        }

        public void PurchaseItem(PurchasableItemInfo purchasableItem, int discount)
        {
            InternalPurchaseItem(purchasableItem, discount, null);
        }

        public bool IsItemUsable(string id)
        {
            return IsItemPurchased(id) || IsItemRented(id);
        }

        public ItemCosts ResolvePurchasableItemCost(string id)
        {
            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
            if (purchasableItemStoredData.ServerPurchasableItemInfos.ContainsKey(id))
            {
                return purchasableItemStoredData.ServerPurchasableItemInfos[id];
            }
            PurchasableToken purchasableToken = InfoResolver.Resolve<FortInfo>().Purchase.PurchasableTokens[id];
            if (purchasableToken.NoneLevelBase)
                return ((NoneLevelBasePurchasableItemInfo) purchasableToken.PurchasableItemInfo).Costs;
            return purchasableToken.PurchasableLevelInfo.Costs;
        }

        private void GetParentList(string id, List<NoneLevelBasePurchasableItemInfo> parents)
        {
            PurchasableToken purchasableToken = InfoResolver.Resolve<FortInfo>().Purchase.PurchasableTokens[id];
            if (purchasableToken.Parent != null)
            {
                parents.Add(purchasableToken.Parent);
                GetParentList(purchasableToken.Parent.Id,parents);
            }
        }

        public bool IsItemPurchased(string id)
        {
            List<NoneLevelBasePurchasableItemInfo> parents = new List<NoneLevelBasePurchasableItemInfo>();
            GetParentList(id, parents);
            parents = parents.Where(info => info.ChildrenPurchased).ToList();

            string[] involvedItemIds =
                parents.Select(info => info.Id)
                    .Concat(new[] {id})
                    .SelectMany(s => InfoResolver.Resolve<FortInfo>().Purchase.PurchasableTokens[s].Bundles.Select(info => info.Id))
                    .Concat(parents.Select(info => info.Id).Concat(new[] {id}))
                    .ToArray();
            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
            foreach (string itemId in involvedItemIds)
            {
                PurchasableToken purchasableToken = InfoResolver.Resolve<FortInfo>().Purchase.PurchasableTokens[itemId];
                if (purchasableToken.NoneLevelBase)
                {
                    if (purchasableToken.PurchasableItemInfo.DefaultBought)
                        return true;
                    if (purchasableItemStoredData.PurchasableItems.ContainsKey(itemId))
                        return true;
                }
                else
                {
                    if (purchasableToken.PurchasableItemInfo.DefaultBought)
                        return true;

                    PurchasableLevelInfo[] purchasableLevelInfos = ((LevelBasePurchasableItemInfo)purchasableToken.PurchasableItemInfo).GetPurchasableLevelInfos();
                    for (int i = purchasableToken.Index; i < purchasableLevelInfos.Length; i++)
                    {
                        if (purchasableLevelInfos[i].DefaultBought)
                            return true;
                    }
                    if (
                        purchasableItemStoredData.PurchasableItems.ContainsKey(purchasableToken.PurchasableItemInfo.Id) &&
                        purchasableItemStoredData.PurchasableItems[purchasableToken.PurchasableItemInfo.Id] >=
                        purchasableToken.Index)
                        return true;

                }
            }
            return false;
        }

        public bool IsItemRented(string id)
        {
            List<NoneLevelBasePurchasableItemInfo> parents = new List<NoneLevelBasePurchasableItemInfo>();
            GetParentList(id, parents);
            parents = parents.Where(info => info.ChildrenPurchased).ToList();

            string[] involvedItemIds =
                parents.Select(info => info.Id)
                    .Concat(new[] { id })
                    .SelectMany(s => InfoResolver.Resolve<FortInfo>().Purchase.PurchasableTokens[s].Bundles.Select(info => info.Id))
                    .Concat(parents.Select(info => info.Id).Concat(new[] { id }))
                    .ToArray();
            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
            foreach (string itemId in involvedItemIds)
            {
                if (purchasableItemStoredData.Rents.ContainsKey(itemId) && purchasableItemStoredData.Rents[itemId] >= DateTime.Now)
                    return true;
            }
            return false;
        }


        public bool IsEnoughFundToPurchaseItem(PurchasableItemInfo purchasableItem, int discount)
        {
            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
/*            PurchasableItemCache purchasableItemCache =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemCache>() ??
                new PurchasableItemCache();*/
            NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo =
                purchasableItem as NoneLevelBasePurchasableItemInfo;
            if (noneLevelBasePurchasableItemInfo != null)
            {
                if (purchasableItemStoredData.PurchasableItems.ContainsKey(noneLevelBasePurchasableItemInfo.Id))
                    throw new Exception("Item already purchased");
                Balance balance = ServiceLocator.Resolve<IUserManagementService>().Balance;
                Balance cost = ResolvePurchasableItemCost(noneLevelBasePurchasableItemInfo.Id).Purchase * discount / 100f;
                if (cost > balance)
                    return false;
                return true;
            }
            else
            {
                LevelBasePurchasableItemInfo levelBasePurchasableItemInfo =
                    (LevelBasePurchasableItemInfo)purchasableItem;
                Array levelInfo =
                    (Array)
                        levelBasePurchasableItemInfo.GetType()
                            .GetProperty("LevelInfo")
                            .GetValue(purchasableItem, new object[0]);
                if (levelInfo == null || levelInfo.Length == 0)
                    throw new Exception("No levels info defined for this purchasable item");
                if (purchasableItemStoredData.PurchasableItems.ContainsKey(levelBasePurchasableItemInfo.Id) &&
                    purchasableItemStoredData.PurchasableItems[levelBasePurchasableItemInfo.Id] >= levelInfo.Length - 1)
                    throw new Exception("No new Level exits in this purchasable item to purchase");
                PurchasableLevelInfo[] purchasableLevelInfos = levelInfo.Cast<PurchasableLevelInfo>().ToArray();

                int purchaseLevelIndex = 0;
                if (purchasableItemStoredData.PurchasableItems.ContainsKey(levelBasePurchasableItemInfo.Id))
                    purchaseLevelIndex = purchasableItemStoredData.PurchasableItems[levelBasePurchasableItemInfo.Id] + 1;

                PurchasableLevelInfo purchasableLevelInfo = purchasableLevelInfos[purchaseLevelIndex];
                Balance balance = ServiceLocator.Resolve<IUserManagementService>().Balance;
                Balance cost = ResolvePurchasableItemCost(purchasableLevelInfo.Id).Purchase * discount / 100f;
                if (cost > balance)
                    return false;
                return true;

            }
        }

        public void SetDiscount(Type packageType, int discount, TimeSpan duration)
        {
            DiscountSavedData discountSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<DiscountSavedData>() ?? new DiscountSavedData();
            discountSavedData.DiscountTokens[packageType] = new DiscountToken
            {
                Unlimited = duration == TimeSpan.MaxValue,
                Discount = discount
            };
            if (!discountSavedData.DiscountTokens[packageType].Unlimited)
                discountSavedData.DiscountTokens[packageType].ExpireDate = DateTime.Now + duration;
        }

        public int GetDiscount(Type packageType)
        {
            DiscountSavedData discountSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<DiscountSavedData>() ?? new DiscountSavedData();
            if (!discountSavedData.DiscountTokens.ContainsKey(packageType))
                return 0;
            if (discountSavedData.DiscountTokens[packageType].Unlimited)
                return discountSavedData.DiscountTokens[packageType].Discount;
            if (discountSavedData.DiscountTokens[packageType].ExpireDate >= DateTime.Now)
                return discountSavedData.DiscountTokens[packageType].Discount;
            return 0;
        }

        public void RemoveDiscount(Type packageType)
        {
            DiscountSavedData discountSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<DiscountSavedData>() ?? new DiscountSavedData();
            if (discountSavedData.DiscountTokens.ContainsKey(packageType))
            {
                discountSavedData.DiscountTokens.Remove(packageType);
            }
        }

        public ErrorPromise<PurchasePackageErrorResult> PurchasePackage(IapPackageInfo iapPackage)
        {
            ErrorDeferred<PurchasePackageErrorResult> deferred = new ErrorDeferred<PurchasePackageErrorResult>();
            if (_isPurchasingPackage)
            {
                deferred.Reject(PurchasePackageErrorResult.AnotherPurchaseInAction);
                return deferred.Promise();
            }
            _isPurchasingPackage = true;
            IMarketProvider marketProvider = GetMarketProvider();
            string payload = Guid.NewGuid().ToString();
            marketProvider.PurchasePackage(iapPackage.Sku, payload).Then(purchaseToken =>
            {
                if (InfoResolver.Resolve<FortInfo>().ServerConnectionProvider != null)
                {
                    PackagePurchaseCache packagePurchaseCache =
                        ServiceLocator.Resolve<IStorageService>().ResolveData<PackagePurchaseCache>() ??
                        new PackagePurchaseCache();
                    packagePurchaseCache.PurchaseTokens[purchaseToken] = new PackageCache
                    {
                        Applied = false,
                        Payload = payload,
                        PurchaseTime = DateTime.Now
                    };
                    ServiceLocator.Resolve<IStorageService>().UpdateData(packagePurchaseCache);
                    ServiceLocator.Resolve<IServerService>()
                        .Call<PurchaseIapPackageResult>("PurchaseIapPackage", new PurchaseIapPackageData
                        {
                            Market = InfoResolver.Resolve<FortInfo>().ActiveMarket,
                            Payload = payload,
                            PurchaseToken = purchaseToken,
                            Sku = iapPackage.Sku
                        }).Then(result =>
                        {
                            if (result.MarketSuccess)
                            {
                                packagePurchaseCache.PurchaseTokens[purchaseToken].Applied = true;
                                packagePurchaseCache.PurchaseTokens[purchaseToken].MarketFailed = false;
                                ServiceLocator.Resolve<IStorageService>().UpdateData(packagePurchaseCache);
                                /*                            UserInfo userInfo = ServiceLocator.Resolve<IStorageService>().ResolveData<UserInfo>() ??
                                                                            new UserInfo();
                                                        userInfo.Balance.SyncValues();
                                                        userInfo.Balance += result.AddedValue;
                                                        ServiceLocator.Resolve<IStorageService>().UpdateData(userInfo);*/
                                _isPurchasingPackage = false;
                                ServiceLocator.Resolve<IAnalyticsService>()
                                    .StatIapPurchased(iapPackage, InfoResolver.Resolve<FortInfo>().ActiveMarket);
                                ApplyIapPackageInfo(iapPackage)
                                    .Then(() => { deferred.Resolve(); }, () => { deferred.Resolve(); });
                                ServiceLocator.Resolve<IEventAggregatorService>().GetEvent<IapPackagePurchasedEvent>().Publish(new IapPackagePurchasedEventArgs(iapPackage));
                            }
                            else
                            {
                                packagePurchaseCache.PurchaseTokens[purchaseToken].Applied = true;
                                packagePurchaseCache.PurchaseTokens[purchaseToken].MarketFailed = true;
                                ServiceLocator.Resolve<IStorageService>().UpdateData(packagePurchaseCache);
                                _isPurchasingPackage = false;
                                ServiceLocator.Resolve<IAnalyticsService>()
                                    .StatIapFailed(iapPackage, purchaseToken,
                                        InfoResolver.Resolve<FortInfo>().ActiveMarket,
                                        IapPurchaseFail.FraudDetected);
                                deferred.Reject(PurchasePackageErrorResult.MarketFailed);
                            }
                        }, () =>
                        {
                            packagePurchaseCache.PurchaseTokens[purchaseToken].Applied = false;
                            packagePurchaseCache.PurchaseTokens[purchaseToken].MarketFailed = false;
                            ServiceLocator.Resolve<IStorageService>().UpdateData(packagePurchaseCache);
                            _isPurchasingPackage = false;
                            ServiceLocator.Resolve<IAnalyticsService>()
                                .StatIapFailed(iapPackage, purchaseToken, InfoResolver.Resolve<FortInfo>().ActiveMarket,
                                    IapPurchaseFail.FortServerFail);
                            deferred.Reject(PurchasePackageErrorResult.Failed);
                        });
                }
                else
                {
                    ApplyIapPackageInfo(iapPackage).Then(() => deferred.Resolve(),() => deferred.Resolve());
                    ServiceLocator.Resolve<IEventAggregatorService>().GetEvent<IapPackagePurchasedEvent>().Publish(new IapPackagePurchasedEventArgs(iapPackage));
                }

            }, error =>
            {
                _isPurchasingPackage = false;
                switch (error)
                {
                    case MarketPurchaseError.Cancel:
                        ServiceLocator.Resolve<IAnalyticsService>()
                            .StatIapFailed(iapPackage, string.Empty, InfoResolver.Resolve<FortInfo>().ActiveMarket,
                                IapPurchaseFail.Cancel);
                        deferred.Reject(PurchasePackageErrorResult.Canceled);
                        break;
                    case MarketPurchaseError.Failed:
                        ServiceLocator.Resolve<IAnalyticsService>()
                            .StatIapFailed(iapPackage, string.Empty, InfoResolver.Resolve<FortInfo>().ActiveMarket,
                                IapPurchaseFail.MarketFailed);
                        deferred.Reject(PurchasePackageErrorResult.Failed);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("error", error, null);
                }
            });
            return deferred.Promise();
        }

        public ErrorPromise<PurchasePackageErrorResult> ReportPurchasePackage(IapPackageInfo iapPackage,
            string purchaseToken)
        {
            if(InfoResolver.Resolve<FortInfo>().ServerConnectionProvider == null)
                throw new Exception("Report purchase package only supported on server mode of fort");
            ErrorDeferred<PurchasePackageErrorResult> deferred = new ErrorDeferred<PurchasePackageErrorResult>();
            PackagePurchaseCache packagePurchaseCache =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PackagePurchaseCache>() ??
                new PackagePurchaseCache();
            ServiceLocator.Resolve<IServerService>()
                .Call<PurchaseIapPackageResult>("PurchaseIapPackage", new PurchaseIapPackageData
                {
                    Market = InfoResolver.Resolve<FortInfo>().ActiveMarket,
                    Payload = "Report:" + Guid.NewGuid(),
                    PurchaseToken = purchaseToken,
                    Sku = iapPackage.Sku
                }).Then(result =>
                {
                    if (result.MarketSuccess)
                    {
                        if (packagePurchaseCache.PurchaseTokens.ContainsKey(purchaseToken))
                        {
                            packagePurchaseCache.PurchaseTokens[purchaseToken].Applied = true;
                            packagePurchaseCache.PurchaseTokens[purchaseToken].MarketFailed = false;
                            ServiceLocator.Resolve<IStorageService>().UpdateData(packagePurchaseCache);
                        }
                        UserInfo userInfo = ServiceLocator.Resolve<IStorageService>().ResolveData<UserInfo>() ??
                                            new UserInfo();
                        userInfo.Balance.SyncValues();
                        userInfo.Balance += result.AddedValue;
                        ServiceLocator.Resolve<IStorageService>().UpdateData(userInfo);
                        _isPurchasingPackage = false;
                        ServiceLocator.Resolve<IAnalyticsService>()
                            .StatIapRetry(iapPackage, purchaseToken, InfoResolver.Resolve<FortInfo>().ActiveMarket);
                        ApplyIapPackageInfo(iapPackage)
                            .Then(() => { deferred.Resolve(); }, () => { deferred.Resolve(); });
                        ServiceLocator.Resolve<IEventAggregatorService>().GetEvent<IapPackagePurchasedEvent>().Publish(new IapPackagePurchasedEventArgs(iapPackage));
                    }
                    else
                    {
                        if (packagePurchaseCache.PurchaseTokens.ContainsKey(purchaseToken))
                        {
                            packagePurchaseCache.PurchaseTokens[purchaseToken].Applied = true;
                            packagePurchaseCache.PurchaseTokens[purchaseToken].MarketFailed = true;
                            ServiceLocator.Resolve<IStorageService>().UpdateData(packagePurchaseCache);
                        }
                        _isPurchasingPackage = false;
                        ServiceLocator.Resolve<IAnalyticsService>()
                            .StatIapRetryFail(iapPackage, purchaseToken, InfoResolver.Resolve<FortInfo>().ActiveMarket,
                                IapRetryFail.FraudDetected);
                        deferred.Reject(PurchasePackageErrorResult.MarketFailed);
                    }
                }, () =>
                {
                    packagePurchaseCache.PurchaseTokens[purchaseToken].Applied = false;
                    packagePurchaseCache.PurchaseTokens[purchaseToken].MarketFailed = false;
                    ServiceLocator.Resolve<IStorageService>().UpdateData(packagePurchaseCache);
                    _isPurchasingPackage = false;
                    ServiceLocator.Resolve<IAnalyticsService>()
                        .StatIapRetryFail(iapPackage, purchaseToken, InfoResolver.Resolve<FortInfo>().ActiveMarket,
                            IapRetryFail.FortServerFail);
                    deferred.Reject(PurchasePackageErrorResult.Failed);
                });
            return deferred.Promise();
        }

        public ComplitionPromise<IapPackageInfo[]> ResolvePackages()
        {
            ComplitionDeferred<IapPackageInfo[]> deferred = new ComplitionDeferred<IapPackageInfo[]>();
            if (InfoResolver.Resolve<FortInfo>().ServerConnectionProvider == null)
            {
                IapPackageInfo[] iapPackageInfos =
                    InfoResolver.Resolve<FortInfo>().Package.Packages.Where(info => info != null).ToArray();
                DiscountIapPackage[] discountIapPackages = iapPackageInfos.OfType<DiscountIapPackage>().ToArray();
                iapPackageInfos = iapPackageInfos.Except(discountIapPackages).Select(info =>
                {
                    IapPackageInfo result;
                    int discount = GetDiscount(info.GetType());
                    DiscountIapPackage discountIapPackage =
                        discountIapPackages.FirstOrDefault(
                            package =>
                                package.PackageData.Discount == discount &&
                                package.PackageData.IapPackageInfo.Sku == info.Sku);
                    if (discount > 0 && discountIapPackage != null)
                    {
                        result = discountIapPackage;
                    }
                    else
                    {
                        result = info;
                    }
                    return result;
                }).ToArray();
                deferred.Resolve(iapPackageInfos);
            }
            else
            {
                ServiceLocator.Resolve<IServerService>().Call<ServerPackage[]>("GetIapPackages").Then(packages =>
                {
                    IapPackageInfo[] iapPackageInfos = ResolveIapPackageInfo(
                        packages.Where(
                            package =>
                                package.Markets == null || package.Markets.Length == 0 ||
                                package.Markets.Contains(InfoResolver.Resolve<FortInfo>().ActiveMarket)).ToArray())
                        .Where(info => info != null)
                        .ToArray();
                    DiscountIapPackage[] discountIapPackages = iapPackageInfos.OfType<DiscountIapPackage>().ToArray();
                    iapPackageInfos = iapPackageInfos.Except(discountIapPackages).Select(info =>
                    {
                        IapPackageInfo result;
                        int discount = GetDiscount(info.GetType());
                        DiscountIapPackage discountIapPackage =
                            discountIapPackages.FirstOrDefault(
                                package =>
                                    package.PackageData.Discount == discount &&
                                    package.PackageData.IapPackageInfo.Sku == info.Sku);
                        if (discount > 0 && discountIapPackage != null)
                        {
                            result = discountIapPackage;
                        }
                        else
                        {
                            result = info;
                        }
                        return result;
                    }).ToArray();
                    deferred.Resolve(iapPackageInfos);
                }, () => deferred.Reject());
            }
            return deferred.Promise();
        }

        #endregion

        #region Private Methods

        private IMarketProvider GetMarketProvider()
        {
            if (_marketProvider == null)
            {
                GameObject marketGameObject = new GameObject("Market");
                DontDestroyOnLoad(marketGameObject);
                MarketInfo marketInfo = FortInfo.Instance.MarketInfos.First(info => info.MarketName == InfoResolver.Resolve<FortInfo>().ActiveMarket);
                _marketProvider = (IMarketProvider) marketGameObject.AddComponent(marketInfo.MarketProvider);
            }
            return _marketProvider;
        }

        internal void InternalPurchaseItem(PurchasableItemInfo purchasableItem, int discount, int? level)
        {
            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
            PurchasableItemCache purchasableItemCache =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemCache>() ??
                new PurchasableItemCache();
            NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo =
                purchasableItem as NoneLevelBasePurchasableItemInfo;
            if (noneLevelBasePurchasableItemInfo != null)
            {
                if (purchasableItemStoredData.PurchasableItems.ContainsKey(noneLevelBasePurchasableItemInfo.Id))
                    throw new Exception("Item already purchased");
                Balance balance = ServiceLocator.Resolve<IUserManagementService>().Balance;
                Balance cost = ResolvePurchasableItemCost(noneLevelBasePurchasableItemInfo.Id).Purchase*discount/100f;
                if (cost > balance)
                    throw new Exception("Insufficient funds");

                ServiceLocator.Resolve<IUserManagementService>().AddScoreAndBalance(0, -cost);
                ValuePurchasableItemInfo valuePurchasableItemInfo = purchasableItem as ValuePurchasableItemInfo;
                if (valuePurchasableItemInfo != null)
                    ServiceLocator.Resolve<IUserManagementService>()
                        .AddScoreAndBalance(0, valuePurchasableItemInfo.Values);
                purchasableItemStoredData.PurchasableItems[noneLevelBasePurchasableItemInfo.Id] = 0;
                purchasableItemCache.ServerPurchasableitemIds[noneLevelBasePurchasableItemInfo.Id] = false;
                ServiceLocator.Resolve<IAnalyticsService>().StatItemPurchased(purchasableItem.Id, cost, discount);
                ServiceLocator.Resolve<IEventAggregatorService>().GetEvent<ItemPurchasedEvent>().Publish(new NoneLevelBaseItemPurchasedEventArgs((NoneLevelBasePurchasableItemInfo) purchasableItem,cost));
            }
            else
            {
                LevelBasePurchasableItemInfo levelBasePurchasableItemInfo =
                    (LevelBasePurchasableItemInfo) purchasableItem;
                Array levelInfo =
                    (Array)
                        levelBasePurchasableItemInfo.GetType()
                            .GetProperty("LevelInfo")
                            .GetValue(purchasableItem, new object[0]);
                if (levelInfo == null || levelInfo.Length == 0)
                    throw new Exception("No levels info defined for this purchasable item");
                if (purchasableItemStoredData.PurchasableItems.ContainsKey(levelBasePurchasableItemInfo.Id) &&
                    purchasableItemStoredData.PurchasableItems[levelBasePurchasableItemInfo.Id] >= levelInfo.Length - 1)
                    throw new Exception("No new Level exits in this purchasable item to purchase");
                PurchasableLevelInfo[] purchasableLevelInfos = levelInfo.Cast<PurchasableLevelInfo>().ToArray();

                int purchaseLevelIndex = 0;
                if (purchasableItemStoredData.PurchasableItems.ContainsKey(levelBasePurchasableItemInfo.Id))
                    purchaseLevelIndex = purchasableItemStoredData.PurchasableItems[levelBasePurchasableItemInfo.Id] + 1;
                if (level != null && level.Value > purchaseLevelIndex)
                    purchaseLevelIndex = level.Value;
                PurchasableLevelInfo purchasableLevelInfo = purchasableLevelInfos[purchaseLevelIndex];
                Balance balance = ServiceLocator.Resolve<IUserManagementService>().Balance;
                Balance cost = ResolvePurchasableItemCost(purchasableLevelInfo.Id).Purchase*discount/100f;
                if (cost > balance)
                    throw new Exception("Insufficient funds");
                ServiceLocator.Resolve<IUserManagementService>().AddScoreAndBalance(0, -cost);
                purchasableItemStoredData.PurchasableItems[levelBasePurchasableItemInfo.Id] = purchaseLevelIndex;
                purchasableItemCache.ServerPurchasableitemIds[purchasableLevelInfo.Id] = false;
                ServiceLocator.Resolve<IAnalyticsService>().StatItemPurchased(purchasableLevelInfo.Id, cost, discount);
                ServiceLocator.Resolve<IEventAggregatorService>().GetEvent<ItemPurchasedEvent>().Publish(new LevelBaseItemPurchasedEventArgs((LevelBasePurchasableItemInfo) purchasableItem, purchaseLevelIndex,cost));
            }
            ServiceLocator.Resolve<IStorageService>().UpdateData(purchasableItemStoredData);
            ServiceLocator.Resolve<IStorageService>().UpdateData(purchasableItemCache);
        }

        private bool IsNoneLevelBaseItemPurchased(NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo)
        {
            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
            return purchasableItemStoredData.PurchasableItems.ContainsKey(noneLevelBasePurchasableItemInfo.Id);
        }

        private int GetLevelBaseItemPurchasedIndex(LevelBasePurchasableItemInfo levelBasePurchasableItemInfo)
        {
            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
            if (!purchasableItemStoredData.PurchasableItems.ContainsKey(levelBasePurchasableItemInfo.Id))
                return -1;
            return purchasableItemStoredData.PurchasableItems[levelBasePurchasableItemInfo.Id];
        }

        private static bool IsSingleItemUsable(string id)
        {
            PurchasableItemStoredData purchasableItemStoredData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<PurchasableItemStoredData>() ??
                new PurchasableItemStoredData();
            PurchasableToken purchasableToken = InfoResolver.Resolve<FortInfo>().Purchase.PurchasableTokens[id];
            if (purchasableToken.NoneLevelBase)
            {
                NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo =
                    (NoneLevelBasePurchasableItemInfo) purchasableToken.PurchasableItemInfo;
                return purchasableItemStoredData.PurchasableItems.ContainsKey(noneLevelBasePurchasableItemInfo.Id) ||
                       (purchasableItemStoredData.Rents.ContainsKey(noneLevelBasePurchasableItemInfo.Id) &&
                        purchasableItemStoredData.Rents[noneLevelBasePurchasableItemInfo.Id] <= DateTime.Now);
            }
            LevelBasePurchasableItemInfo levelBasePurchasableItemInfo =
                (LevelBasePurchasableItemInfo) purchasableToken.PurchasableItemInfo;
            if (purchasableItemStoredData.PurchasableItems.ContainsKey(levelBasePurchasableItemInfo.Id) &&
                purchasableItemStoredData.PurchasableItems[levelBasePurchasableItemInfo.Id] >= purchasableToken.Index)
                return true;
            return purchasableItemStoredData.Rents.ContainsKey(id) &&
                   purchasableItemStoredData.Rents[id] <= DateTime.Now;
        }

        private Promise ApplyIapPackageInfo(IapPackageInfo iapPackage)
        {
            DiscountIapPackage discountIapPackage = iapPackage as DiscountIapPackage;
            if (discountIapPackage != null)
            {
                iapPackage = discountIapPackage.PackageData.IapPackageInfo;
            }
            ValueIapPackage valueIapPackage = iapPackage as ValueIapPackage;
            if (valueIapPackage != null)
            {
                ServiceLocator.Resolve<IUserManagementService>()
                    .AddScoreAndBalance(0, valueIapPackage.PackageData.Values);
            }
            PurchasableItemsIapPackageInfo purchasableItemsIapPackageInfo = iapPackage as PurchasableItemsIapPackageInfo;
            if (purchasableItemsIapPackageInfo != null)
            {
                foreach (PurchaseData purchaseData in purchasableItemsIapPackageInfo.PackageData.PurchaseDatas)
                {
                    try
                    {
                        PurchaseLevelBaseData purchaseLevelBaseData = purchaseData as PurchaseLevelBaseData;
                        if (purchaseLevelBaseData != null)
                        {
                            InternalPurchaseItem(purchaseLevelBaseData.PurchasableItemInfo, 100,
                                purchaseLevelBaseData.Level);
                        }
                        PurchaseNoneLevelBaseData purchaseNoneLevelBaseData = purchaseData as PurchaseNoneLevelBaseData;
                        if (purchaseNoneLevelBaseData != null)
                        {
                            InternalPurchaseItem(purchaseNoneLevelBaseData.PurchasableItemInfo, 100, null);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            SkinnerBoxIapPackageInfo skinnerBoxIapPackageInfo = iapPackage as SkinnerBoxIapPackageInfo;
            if (skinnerBoxIapPackageInfo != null)
            {
                SkinnerBoxSavedData skinnerBoxSavedData =
                    ServiceLocator.Resolve<IStorageService>().ResolveData<SkinnerBoxSavedData>() ??
                    new SkinnerBoxSavedData();

                foreach (SkinnerBoxIap skinnerBoxIap in skinnerBoxIapPackageInfo.PackageData.SkinnerBoxes)
                {
                    int itemCount = 0;
                    if (skinnerBoxSavedData.ItemCount.ContainsKey(skinnerBoxIap.SkinnerBoxInfo.Id))
                    {
                        itemCount = skinnerBoxSavedData.ItemCount[skinnerBoxIap.SkinnerBoxInfo.Id];
                    }
                    itemCount += skinnerBoxIap.Count;
                    skinnerBoxSavedData.ItemCount[skinnerBoxIap.SkinnerBoxInfo.Id] = itemCount;
                }
                ServiceLocator.Resolve<IStorageService>().UpdateData(skinnerBoxSavedData);
            }
            if (iapPackage is RemoveAdsIapPackageInfo)
            {
                ServiceLocator.Resolve<IStorageService>().UpdateData(new AdvertisementSavedData {IsAdRemoved = true});
                if (ServiceLocator.Resolve<IAdvertisementService>().IsStandardBannerSupported)
                    ServiceLocator.Resolve<IAdvertisementService>().HideStandardBanner();
            }
            if (InfoResolver.Resolve<FortInfo>().ServerConnectionProvider == null)
            {
                Deferred deferred = new Deferred();
                deferred.Resolve();
                return deferred.Promise();
            }
            return ServiceLocator.Resolve<IUserManagementService>().FullUpdate();
        }

        private IapPackageInfo[] ResolveIapPackageInfo(ServerPackage[] packages)
        {
            List<IapPackageInfo> result = new List<IapPackageInfo>();
            Dictionary<string, List<Action<IapPackageInfo>>> forwarded =
                new Dictionary<string, List<Action<IapPackageInfo>>>();
            foreach (ServerPackage package in packages)
            {
                Type type = Type.GetType(package.PackageType);
                if (type == null)
                    return null;
                IapPackageInfo iapPackage = (IapPackageInfo) Activator.CreateInstance(type);
                iapPackage.Sku = package.Sku;
                iapPackage.DisplayName = new CustomLanguageItem<string>(InfoResolver.Resolve<FortInfo>().Language.ActiveLanguages.Select(info => info.Id).ToDictionary(s => s,s => package.DisplayName));
                iapPackage.Markets =
                    package.Markets.Select(
                        s => InfoResolver.Resolve<FortInfo>().MarketInfos.FirstOrDefault(info => info.MarketName == s))
                        .Where(info => info != null)
                        .ToArray();
                iapPackage.Price = package.Price;
                PropertyInfo propertyInfo = type.GetProperty("PackageData");
                if (propertyInfo != null)
                {
                    if (propertyInfo.PropertyType == typeof (DiscountIapData))
                    {
                        DiscountIapData discountIapData = new DiscountIapData();
                        discountIapData.Discount = package.PackageData.GetValue("Discount").ToObject<int>();
                        string parentSku = package.PackageData.GetValue("IapPackageInfo").ToObject<string>();
                        if (packages.All(serverPackage => serverPackage.Sku != parentSku))
                            continue;

                        propertyInfo.SetValue(iapPackage, discountIapData, new object[0]);

                        if (!forwarded.ContainsKey(parentSku))
                            forwarded[parentSku] = new List<Action<IapPackageInfo>>();
                        forwarded[parentSku].Add(info => { discountIapData.IapPackageInfo = info; });
                    }
                    else
                    {
                        if (package.PackageData != null)
                        {
                            propertyInfo.SetValue(iapPackage, package.PackageData.ToObject(propertyInfo.PropertyType),
                                new object[0]);
                        }
                    }
                }
                result.Add(iapPackage);
            }
            foreach (KeyValuePair<string, List<Action<IapPackageInfo>>> pair in forwarded)
            {
                IapPackageInfo packageInfo = result.FirstOrDefault(info => info.Sku == pair.Key);
                if (packageInfo == null)
                    continue;
                foreach (Action<IapPackageInfo> action in pair.Value)
                {
                    action(packageInfo);
                }
            }

            return result.ToArray();
        }

        #endregion

        #region Nested types

        [Serializable]
        public class PurchasableItemStoredData
        {
            #region Constructors

            public PurchasableItemStoredData()
            {
                PurchasableItems = new Dictionary<string, int>();
                ServerPurchasableItemInfos = new Dictionary<string, ItemCosts>();
            }

            #endregion

            #region Properties

            public Dictionary<string, DateTime> Rents { get; set; }
            public Dictionary<string, int> PurchasableItems { get; set; }
            public Dictionary<string, ItemCosts> ServerPurchasableItemInfos { get; set; }

            #endregion
        }


        [Serializable]
        public class PurchasableItemCache
        {
            #region Constructors

            public PurchasableItemCache()
            {
                ServerPurchasableitemIds = new Dictionary<string, bool>();
            }

            #endregion

            #region Properties

            public Dictionary<string, bool> ServerPurchasableitemIds { get; set; }

            #endregion
        }

        [Serializable]
        public class PackagePurchaseCache
        {
            #region Constructors

            public PackagePurchaseCache()
            {
                PurchaseTokens = new Dictionary<string, PackageCache>();
            }

            #endregion

            #region Properties

            public Dictionary<string, PackageCache> PurchaseTokens { get; set; }

            #endregion
        }

        [Serializable]
        public class PackageCache
        {
            #region Properties

            public string Payload { get; set; }
            public bool Applied { get; set; }
            public DateTime PurchaseTime { get; set; }
            public bool MarketFailed { get; set; }

            #endregion
        }

        private class PurchaseIapPackageData
        {
            #region Properties

            [JsonProperty("market")]
            public string Market { get; set; }

            [JsonProperty("sku")]
            public string Sku { get; set; }

            [JsonProperty("purchaseToken")]
            public string PurchaseToken { get; set; }

            [JsonProperty("payload")]
            public string Payload { get; set; }

            #endregion
        }

        private class PurchaseIapPackageResult
        {
            #region Properties

            public Balance AddedValue { get; set; }

            public bool MarketSuccess { get; set; }

            #endregion
        }

        private class ServerPackage
        {
            #region Properties

            public string DisplayName { get; set; }
            public string Sku { get; set; }
            public int Price { get; set; }
            public string PackageType { get; set; }
            public string[] Markets { get; set; }
            public JObject PackageData { get; set; }

            #endregion
        }

        private class DiscountSavedData
        {
            #region Constructors

            public DiscountSavedData()
            {
                DiscountTokens = new Dictionary<Type, DiscountToken>();
            }

            #endregion

            #region Properties

            public Dictionary<Type, DiscountToken> DiscountTokens { get; set; }

            #endregion
        }

        private class DiscountToken
        {
            #region Properties

            public bool Unlimited { get; set; }
            public DateTime ExpireDate { get; set; }
            public int Discount { get; set; }

            #endregion
        }

        #endregion
    }
}