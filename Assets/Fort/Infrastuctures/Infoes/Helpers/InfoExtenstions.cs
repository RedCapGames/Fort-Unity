using System.Collections.Generic;
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
    }
}
