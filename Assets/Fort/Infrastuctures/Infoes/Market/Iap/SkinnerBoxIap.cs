using System.Linq;
using System.Reflection;
using Fort.Info.PurchasableItem;
using Fort.Info.SkinnerBox;
using Fort.Inspector;
using Newtonsoft.Json;

namespace Fort.Info.Market.Iap
{
    public class SkinnerBoxIap
    {
        public int Count { get; set; }
        [JsonConverter(typeof(SkinnerBoxJsonConverter))]
        [PropertyInstanceResolve(typeof(SkinnerBoxIapPropertyInstanceResolver))]
        public PurchableSkinnerBoxInfo SkinnerBoxInfo { get; set; }
    }
    public class SkinnerBoxIapPropertyInstanceResolver : IPropertyInstanceResolver
    {
        #region Implementation of IPropertyInstanceResolver

        public InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property)
        {
            InstanceResolverResult result = new InstanceResolverResult
            {
                PossibleInstanceTokens = InfoResolver.Resolve<FortInfo>().SkinnerBox.BoxInfos.OfType<PurchableSkinnerBoxInfo>().Select(info => new InstanceToken(info.Name, info)).ToArray()

            };
            InstanceToken instanceToken =
                result.PossibleInstanceTokens.FirstOrDefault(token => ReferenceEquals(token.Value, data));
            result.PresentableInstanceTokens = instanceToken == null ? new InstanceToken[0] : new[] { instanceToken };
            return result;
        }

        #endregion
    }
}