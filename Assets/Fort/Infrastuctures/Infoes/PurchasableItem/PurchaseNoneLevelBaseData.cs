using System.Linq;
using System.Reflection;
using Fort.Inspector;

namespace Fort.Info.PurchasableItem
{
    public class PurchaseNoneLevelBaseData : PurchaseData
    {
        [PropertyInstanceResolve(typeof(PurchaseNoneLevelBasePropertyInstanceResolver))]
        public NoneLevelBasePurchasableItemInfo PurchasableItemInfo { get; set; }
    }
    public class PurchaseNoneLevelBasePropertyInstanceResolver : IPropertyInstanceResolver
    {
        #region Implementation of IPropertyInstanceResolver

        public InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property)
        {
            InstanceResolverResult result = new InstanceResolverResult
            {
                PossibleInstanceTokens = InfoResolver.FortInfo.Purchase.GetAllPurchasableItemInfos().OfType<NoneLevelBasePurchasableItemInfo>().Select(info => new InstanceToken(info.Name, info)).ToArray()//InfoResolver.FortInfo.Package.Packages.Where(info => !(info is DiscountIapPackage)).Select(info => new InstanceToken(info.Sku, info)).ToArray()

            };
            InstanceToken instanceToken =
                result.PossibleInstanceTokens.FirstOrDefault(token => ReferenceEquals(token.Value, data));
            result.PresentableInstanceTokens = instanceToken == null ? new InstanceToken[0] : new[] { instanceToken };
            return result;
        }

        #endregion
    }
}