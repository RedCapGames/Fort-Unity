using System;
using System.Linq;
using System.Reflection;
using Fort.Info.Language;
using Fort.Inspector;
using Newtonsoft.Json;

namespace Fort.Info.Market.Iap
{
    public abstract class IapPackageInfo
    {
        protected IapPackageInfo()
        {
            Markets = new MarketInfo[0];
        }
        public string Sku { get; set; }
        public LanguageItem<string> DisplayName { get; set; }
        public int Price { get; set; }
        [JsonConverter(typeof(MarketInfoesJsonConverter))]
        [PropertyInstanceResolve(typeof(MarketInfoesPropertyInstanceResolver))]
        public MarketInfo[] Markets { get; set; }
    }
    public abstract class IapPackageInfo<T> : IapPackageInfo
    {
        protected IapPackageInfo()
        {
            PackageData = Activator.CreateInstance<T>();
        }
        public T PackageData { get; set; }
    }

    internal class MarketInfoesPropertyInstanceResolver : IPropertyInstanceResolver
    {
        #region Implementation of IPropertyInstanceResolver

        public InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property)
        {
            MarketInfo[] marketInfos = (MarketInfo[])data;
            if (marketInfos == null)
                marketInfos = new MarketInfo[0];
            MarketInfo[] possibleMarkets = InfoResolver.FortInfo.MarketInfos.Where(info => marketInfos.All(marketInfo => marketInfo.GetType() != info.GetType()) ).ToArray();
            InstanceResolverResult result = new InstanceResolverResult
            {
                PossibleInstanceTokens = possibleMarkets.Select(market => new InstanceToken(market.GetType().Name, market)).ToArray(),
                PresentableInstanceTokens = marketInfos.Select(info => new InstanceToken(info.GetType().Name, info)).ToArray(),
                IsEditable = false,
                UseValueTypeForEdit = false
            };
            return result;
        }

        #endregion
    }
}
