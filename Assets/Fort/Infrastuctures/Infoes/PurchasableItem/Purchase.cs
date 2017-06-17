﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fort.Inspector;
using Newtonsoft.Json;

namespace Fort.Info.PurchasableItem
{
    public class Purchase
    {
        private Dictionary<string, PurchasableToken> _purchasableTokens;

        private PurchasableItemInfo[] _purchasableItemInfos;

        [JsonIgnore]
        [IgnoreProperty]
        public Dictionary<string, PurchasableToken> PurchasableTokens
        {
            get
            {
                if (_purchasableTokens == null)
                {
                    SyncPurchasableTokens();
                }
                return _purchasableTokens;
            }
            //set { _purchasableTokens = value; }
        }


        private void SyncPurchasableTokens()
        {
            _purchasableTokens = new Dictionary<string, PurchasableToken>();
            PurchasableItemInfo[] allPurchasableItemInfos = GetAllPurchasableItemInfos();

            foreach (PurchasableItemInfo purchasableItemInfo in allPurchasableItemInfos.Where(info => info != null))
            {
                if (purchasableItemInfo is NoneLevelBasePurchasableItemInfo)
                {
                    _purchasableTokens.Add(purchasableItemInfo.Id, new PurchasableToken
                    {
                        PurchasableItemInfo = purchasableItemInfo,
                        NoneLevelBase = true
                    });
                }
                else
                {

                    //PropertyInfo propertyInfo = purchasableItemInfo.GetType().GetProperty("LevelInfos");
                    int index = 0;
                    _purchasableTokens.Add(purchasableItemInfo.Id, new PurchasableToken
                    {
                        PurchasableItemInfo = purchasableItemInfo,
                        NoneLevelBase = false,
                        Index = -1
                    });
                    object value = ((LevelBasePurchasableItemInfo)purchasableItemInfo).GetPurchasableLevelInfos();// propertyInfo.GetValue(purchasableItemInfo, new object[0]);
                    if (value != null)
                    {
                        foreach (
                            PurchasableLevelInfo purchasableLevelInfo in
                                ((IEnumerable)value).Cast<PurchasableLevelInfo>())
                        {
                            _purchasableTokens.Add(purchasableLevelInfo.Id, new PurchasableToken
                            {
                                PurchasableItemInfo = purchasableItemInfo,
                                PurchasableLevelInfo = purchasableLevelInfo,
                                Index = index,
                                NoneLevelBase = false
                            });
                            index++;
                        }
                    }
                }
            }
            //Parent assignment
            foreach (KeyValuePair<string, PurchasableToken> pair in _purchasableTokens.ToArray())
            {
                if ((pair.Value.PurchasableItemInfo is NoneLevelBasePurchasableItemInfo) && ((NoneLevelBasePurchasableItemInfo)pair.Value.PurchasableItemInfo).ChildrenPurchasableItems != null)
                    foreach (PurchasableItemInfo childrenPurchasableItem in ((NoneLevelBasePurchasableItemInfo)pair.Value.PurchasableItemInfo).ChildrenPurchasableItems)
                    {
                        if (childrenPurchasableItem is NoneLevelBasePurchasableItemInfo)
                        {
                            _purchasableTokens[childrenPurchasableItem.Id].Parent = (NoneLevelBasePurchasableItemInfo)pair.Value.PurchasableItemInfo;
                        }
                        else
                        {
                            foreach (PurchasableLevelInfo purchasableLevelInfo in ((LevelBasePurchasableItemInfo)childrenPurchasableItem).GetPurchasableLevelInfos())
                            {
                                _purchasableTokens[purchasableLevelInfo.Id].Parent = (NoneLevelBasePurchasableItemInfo)pair.Value.PurchasableItemInfo;
                            }
                        }
                    }
            }
        }


        public PurchasableItemInfo[] PurchasableItemInfos
        {
            get { return _purchasableItemInfos; }
            set
            {
                _purchasableItemInfos = value;
                SyncPurchasableTokens();                
            }
        }

        public PurchasableItemInfo[] GetAllPurchasableItemInfos()
        {
            List<PurchasableItemInfo> result = new List<PurchasableItemInfo>();
            foreach (PurchasableItemInfo purchasableItemInfo in PurchasableItemInfos ?? new PurchasableItemInfo[0])
            {
                InternalGetAllPurchasableItemInfos(result, purchasableItemInfo);
            }
            return result.ToArray();
        }

        private void InternalGetAllPurchasableItemInfos(List<PurchasableItemInfo> items, PurchasableItemInfo purchasableItemInfo)
        {
            if (items.Contains(purchasableItemInfo))
                return;
            items.Add(purchasableItemInfo);
            if (purchasableItemInfo is NoneLevelBasePurchasableItemInfo && ((NoneLevelBasePurchasableItemInfo)purchasableItemInfo).ChildrenPurchasableItems != null)
                foreach (PurchasableItemInfo childrenPurchasableItem in ((NoneLevelBasePurchasableItemInfo)purchasableItemInfo).ChildrenPurchasableItems)
                {
                    InternalGetAllPurchasableItemInfos(items, childrenPurchasableItem);
                }
        }


    }
    public class PurchasableToken
    {
        #region Properties

        public bool NoneLevelBase { get; set; }
        public int Index { get; set; }
        public PurchasableItemInfo PurchasableItemInfo { get; set; }
        public PurchasableLevelInfo PurchasableLevelInfo { get; set; }
        public NoneLevelBasePurchasableItemInfo Parent { get; set; }

        #endregion
    }
}