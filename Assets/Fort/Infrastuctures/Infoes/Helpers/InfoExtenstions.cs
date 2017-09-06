using System;
using System.Collections.Generic;
using System.Linq;
using Fort.Info.GameLevel;
using Fort.Info.PurchasableItem;

namespace Fort.Info
{
    public static class InfoExtenstions
    {
        public static GameLevelCategory GetCategory(this GameLevelInfo gameLevelInfo)
        {
            return FortInfo.Instance.GameLevel.LevelCategoriesParentMap[gameLevelInfo.Id];
        }

        public static GameLevelCategory GetParentCategory(this GameLevelCategory category)
        {
            Dictionary<string, GameLevelCategory> levelCategoriesParentMap = FortInfo.Instance.GameLevel.LevelCategoriesParentMap;
            return !levelCategoriesParentMap.ContainsKey(category.Id) ? null : levelCategoriesParentMap[category.Id];
        }

        public static NoneLevelBasePurchasableItemInfo GetParent(this PurchasableItemInfo purchasableItemInfo)
        {
            return FortInfo.Instance.Purchase.PurchasableTokens[purchasableItemInfo.Id].Parent;
        }

        public static PurchasableItemInfo[] GetItems(this Purchase purchase, Type type)
        {
            return purchase.GetAllPurchasableItemInfos().Where(info => info != null && info.GetType() == type).ToArray();
        }

        public static T[] GetItems<T>(this Purchase purchase)
        {
            return purchase.GetAllPurchasableItemInfos().OfType<T>().ToArray();
        }

        public static PurchasableItemInfo GetItem(this Purchase purchase, Type type)
        {
            return purchase.GetAllPurchasableItemInfos().First(info => info != null && info.GetType() == type);
        }

        public static T GetItem<T>(this Purchase purchase) where T : PurchasableItemInfo
        {
            return (T) GetItem(purchase, typeof (T));
        }
        public static T GetItem<T>(this Purchase purchase,string name) where T : PurchasableItemInfo
        {
            return purchase.GetAllPurchasableItemInfos().OfType<T>().First(arg1 => arg1.Name == name);
        }
        public static PurchasableItemInfo GetItem(this Purchase purchase, string name) 
        {
            return purchase.GetAllPurchasableItemInfos().First(arg1 => arg1.Name == name);
        }

    }
}
