using System;
using Fort.Aggregator;
using Fort.Info;
using Fort.Info.PurchasableItem;

namespace Fort.Events
{
    public class ItemPurchasedEvent:PubSubEvent<ItemPurchasedEventArgs>
    {
    }

    public abstract class ItemPurchasedEventArgs : EventArgs
    {
        protected ItemPurchasedEventArgs(PurchasableItemInfo purchasableItemInfo, Balance cost)
        {
            PurchasableItemInfo = purchasableItemInfo;
            Cost = cost;
        }

        public PurchasableItemInfo PurchasableItemInfo { get; private set; }
        public Balance Cost { get; private set; }
    }
    public class LevelBaseItemPurchasedEventArgs:ItemPurchasedEventArgs
    {
        public LevelBasePurchasableItemInfo LevelBasePurchasableItem { get; private set; }
        public int PurchasedItemIndex { get; private set; }

        public LevelBaseItemPurchasedEventArgs(LevelBasePurchasableItemInfo levelBasePurchasableItem,int purchasedItemIndex,Balance cost) : base(levelBasePurchasableItem,cost)
        {
            LevelBasePurchasableItem = levelBasePurchasableItem;
            PurchasedItemIndex = purchasedItemIndex;
        }
    }
    public class NoneLevelBaseItemPurchasedEventArgs:ItemPurchasedEventArgs
    {
        public NoneLevelBasePurchasableItemInfo NoneLevelBasePurchasableItemInfo { get; private set; }

        public NoneLevelBaseItemPurchasedEventArgs(NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo, Balance cost) : base(noneLevelBasePurchasableItemInfo,cost)
        {
            NoneLevelBasePurchasableItemInfo = noneLevelBasePurchasableItemInfo;
        }
    }
}
