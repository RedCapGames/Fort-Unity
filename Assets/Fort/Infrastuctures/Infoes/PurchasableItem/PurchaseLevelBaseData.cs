using System.Linq;
using System.Reflection;
using Fort.Inspector;

namespace Fort.Info.PurchasableItem
{
    public class PurchaseLevelBaseData: PurchaseData
    {
        [PropertyInstanceResolve(typeof(PurchaseLevelBasePropertyInstanceResolver))]
        public LevelBasePurchasableItemInfo PurchasableItemInfo { get; set; }
        public int Level { get; set; }
    }
    public class PurchaseLevelBasePropertyInstanceResolver : IPropertyInstanceResolver
    {
        #region Implementation of IPropertyInstanceResolver

        public InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property)
        {
            InstanceResolverResult result = new InstanceResolverResult
            {
                PossibleInstanceTokens = InfoResolver.FortInfo.Purchase.GetAllPurchasableItemInfos().OfType<LevelBasePurchasableItemInfo>().Select(info => new InstanceToken(info.Name, info)).ToArray()
            };
            InstanceToken instanceToken =
                result.PossibleInstanceTokens.FirstOrDefault(token => ReferenceEquals(token.Value, data));
            result.PresentableInstanceTokens = instanceToken == null ? new InstanceToken[0] : new[] { instanceToken };
            return result;
        }

        #endregion
    }
}