using System.Linq;
using System.Reflection;
using Fort.Inspector;
using Newtonsoft.Json;

namespace Fort.Info.Market.Iap
{
    public class DiscountIapData
    {
        [JsonConverter(typeof(DiscountSkuJsonConverter))]
        [PropertyInstanceResolve(typeof(DiscountIapPackagePropertyInstanceResolver))]
        public IapPackageInfo IapPackageInfo { get; set; }
        public int Discount { get; set; }

    }
    public class DiscountIapPackagePropertyInstanceResolver : IPropertyInstanceResolver
    {
        #region Implementation of IPropertyInstanceResolver

        public InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property)
        {
            InstanceResolverResult result = new InstanceResolverResult
            {
                PossibleInstanceTokens = InfoResolver.Resolve<FortInfo>().Package.Packages.Where(info => !(info is DiscountIapPackage)).Select(info => new InstanceToken(info.Sku, info)).ToArray()

            };
            InstanceToken instanceToken =
                result.PossibleInstanceTokens.FirstOrDefault(token => ReferenceEquals(token.Value,data));
            result.PresentableInstanceTokens = instanceToken == null ? new InstanceToken[0] : new[] { instanceToken };
            return result;
        }

        #endregion
    }
}