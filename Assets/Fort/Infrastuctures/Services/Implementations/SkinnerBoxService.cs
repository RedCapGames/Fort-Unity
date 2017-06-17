using System;
using System.Collections.Generic;
using System.Linq;
using Fort.Info;
using Fort.Info.SkinnerBox;

namespace Fort
{
    [Service(ServiceType = typeof(ISkinnerBoxService))]
    public class SkinnerBoxService:ISkinnerBoxService
    {
        #region Implementation of ISkinnerBoxService

        public bool IsFreeSkinnerBoxAvailable(FreeSkinnerBoxInfo boxInfo)
        {
            SkinnerBoxSavedData skinnerBoxSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<SkinnerBoxSavedData>() ??
                new SkinnerBoxSavedData();
            if (!skinnerBoxSavedData.FreeItemUseTime.ContainsKey(boxInfo.Id))
                return true;
            if (skinnerBoxSavedData.FreeItemUseTime[boxInfo.Id] < DateTime.Now)
                return true;
            return false;
        }

        public int GetPurchableskinnerBoxCount(PurchableSkinnerBoxInfo boxInfo)
        {
            SkinnerBoxSavedData skinnerBoxSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<SkinnerBoxSavedData>() ??
                new SkinnerBoxSavedData();
            if (!skinnerBoxSavedData.ItemCount.ContainsKey(boxInfo.Id))
                return 0;
            return skinnerBoxSavedData.ItemCount[boxInfo.Id];
        }

        private SkinnerBoxItemInfo PickItem(SkinnerBoxItemInfo[] items)
        {
            int total = items.Sum(info => info.Chance == 0 ? 1 : info.Chance);
            Random random = new Random();
            int rand = random.Next(total);
            int counter = 0;
            SkinnerBoxItemInfo result = items.First();
            foreach (SkinnerBoxItemInfo boxItemInfo in items)
            {
                counter += (boxItemInfo.Chance == 0 ? 1 : boxItemInfo.Chance);
                if (rand < counter)
                {
                    result = boxItemInfo;
                    break;
                }
            }
            return result;
        }

        private void ApplySkinnerBoxItemInfo(SkinnerBoxItemInfo skinnerBoxItemInfo)
        {
            ValueSkinnerBoxItemInfo valueSkinnerBoxItemInfo = skinnerBoxItemInfo as ValueSkinnerBoxItemInfo;
            if (valueSkinnerBoxItemInfo != null)
            {
                ServiceLocator.Resolve<IUserManagementService>().AddScoreAndBalance(0,valueSkinnerBoxItemInfo.Value);
            }
            PurchasableItemSkinnerBoxItemInfo purchasableItemSkinnerBoxItemInfo = skinnerBoxItemInfo as PurchasableItemSkinnerBoxItemInfo;
            if (purchasableItemSkinnerBoxItemInfo != null)
            {
                //purchasableItemSkinnerBoxItemInfo.PurchasableItemInfo
            }
        }

        public SkinnerBoxItemInfo OpenBox(SkinnerBoxInfo boxInfo)
        {
            SkinnerBoxSavedData skinnerBoxSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<SkinnerBoxSavedData>() ??
                new SkinnerBoxSavedData();
            FreeSkinnerBoxInfo freeSkinnerBoxInfo = boxInfo as FreeSkinnerBoxInfo;
            if (freeSkinnerBoxInfo != null)
            {
                if(!IsFreeSkinnerBoxAvailable(freeSkinnerBoxInfo))
                    throw new Exception("no free skinner box avialable.");
                SkinnerBoxItemInfo skinnerBoxItemInfo = PickItem(freeSkinnerBoxInfo.Items);
                skinnerBoxSavedData.FreeItemUseTime[boxInfo.Id] = DateTime.Now +
                                                                  TimeSpan.FromSeconds(freeSkinnerBoxInfo.UseDelay);
                ServiceLocator.Resolve<IStorageService>().UpdateData(skinnerBoxSavedData);
                ApplySkinnerBoxItemInfo(skinnerBoxItemInfo);
                return skinnerBoxItemInfo;
            }
            PurchableSkinnerBoxInfo purchableSkinnerBoxInfo = boxInfo as PurchableSkinnerBoxInfo;
            if (purchableSkinnerBoxInfo != null)
            {
                if (GetPurchableskinnerBoxCount(purchableSkinnerBoxInfo)<=0)
                    throw new Exception("no purchasable skinner box avialable.");
                SkinnerBoxItemInfo skinnerBoxItemInfo = PickItem(purchableSkinnerBoxInfo.Items);
                skinnerBoxSavedData.ItemCount[purchableSkinnerBoxInfo.Id]--;
                ServiceLocator.Resolve<IStorageService>().UpdateData(skinnerBoxSavedData);
                ApplySkinnerBoxItemInfo(skinnerBoxItemInfo);
                return skinnerBoxItemInfo;
            }
            return null;
        }

        public TimeSpan GetFreeSkinnerBoxAvailabiltyDuration(FreeSkinnerBoxInfo boxInfo)
        {
            SkinnerBoxSavedData skinnerBoxSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<SkinnerBoxSavedData>() ??
                new SkinnerBoxSavedData();
            if (!skinnerBoxSavedData.FreeItemUseTime.ContainsKey(boxInfo.Id))
                return TimeSpan.Zero;
            if (skinnerBoxSavedData.FreeItemUseTime[boxInfo.Id] <= DateTime.Now)
                return TimeSpan.Zero;
            return DateTime.Now - skinnerBoxSavedData.FreeItemUseTime[boxInfo.Id];
        }

        #endregion
    }
    public class SkinnerBoxSavedData
    {
        public SkinnerBoxSavedData()
        {
            ItemCount = new Dictionary<string, int>();
            FreeItemUseTime = new Dictionary<string, DateTime>();
        }
        public Dictionary<string, int> ItemCount { get; set; }
        public Dictionary<string,DateTime> FreeItemUseTime { get; set; }
    }
}
