using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Inspector;

namespace Fort.Info
{
    public class SkinnerBox
    {
        public SkinnerBoxInfo[] BoxInfos { get; set; }
    }
    public abstract class SkinnerBoxInfo
    {
        protected SkinnerBoxInfo()
        {
            Id = Guid.NewGuid().ToString();
        }
        [IgnoreProperty]
        public string Id { get; set; }

        public SkinnerBoxItemInfo[] Items { get; set; }
    }

    public class FreeSkinnerBoxInfo : SkinnerBoxInfo
    {
        public float UseDelay { get; set; }
    }

    public class PurchableSkinnerBoxInfo : SkinnerBoxInfo
    {
        
    }

    public abstract class SkinnerBoxItemInfo
    {
        public int Chance { get; set; }
    }

    public class ValueSkinnerBoxItemInfo : SkinnerBoxItemInfo
    {
        public Balance Value { get; set; }
    }

    public class PurchasableItemSkinnerBoxItemInfo : SkinnerBoxItemInfo
    {
        public PurchasableItemInfo PurchasableItemInfo { get; set; }
    }

    public class RentPurchasableItemSkinnerBoxItemInfo : PurchasableItemSkinnerBoxItemInfo
    {
        public float RentDuration { get; set; }
    }

}
