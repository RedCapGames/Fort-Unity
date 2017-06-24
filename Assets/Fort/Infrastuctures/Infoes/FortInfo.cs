using System;
using System.Linq;
using System.Reflection;
using Fort.Info;
using Fort.Info.Achievement;
using Fort.Info.Analytics;
using Fort.Info.GameLevel;
using Fort.Info.Invitation;
using Fort.Info.Language;
using Fort.Info.Market;
using Fort.Info.Market.Iap;
using Fort.Info.PurchasableItem;
using Fort.Info.SkinnerBox;
using Fort.ServerConnection;
using UnityEngine;

namespace Fort.Inspector
{
    public class FortInfo
    {
        public FortInfo()
        {
            ValueDefenitions = new string[0];
            MarketInfos = new MarketInfo[0];
            Package = new IapPackage();
            InvitationInfo = new InvitationInfo();
            Achievement = new Achievement();
            Purchase = new Purchase();
            GameLevel = new GameLevel();
            Analytic = new Analytic();
            Advertisement = new Info.Advertisement.Advertisement();
            SkinnerBox = new SkinnerBox();
            Language = new FortLanguage();
        }

        public IServerConnectionProvider ServerConnectionProvider { get; set; }
        public FortLanguage Language { get; set; }

        [Inspector(Presentation = "Fort.CustomEditor.ValueDefenitionsPresenter")]

        public string[] ValueDefenitions { get; set; }
        [PropertyInstanceResolve(typeof(MarketInfoesPropertyInstanceResolver))]
        public MarketInfo[] MarketInfos { get; set; }
        public IapPackage Package { get; set; }
        [PropertyInstanceResolve(typeof(ActiveMarketPropertyInstanceResolver))]
        public string ActiveMarket { get; set; }
        public InvitationInfo InvitationInfo { get; set; }
        public Achievement Achievement { get; set; }
        public Purchase Purchase { get; set; }
        public GameLevel GameLevel { get; set; }
        public Analytic Analytic { get; set; }
        public Info.Advertisement.Advertisement Advertisement { get; set; }
        public SkinnerBox SkinnerBox { get; set; }        
    }
    public class ActiveMarketPropertyInstanceResolver : IPropertyInstanceResolver
    {
        #region Implementation of IPropertyInstanceResolver

        public InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property)
        {
            InstanceResolverResult result = new InstanceResolverResult
            {
                PossibleInstanceTokens = InfoResolver.FortInfo.MarketInfos.Select(info => new InstanceToken(info.MarketName, info.MarketName)).ToArray()

            };
            InstanceToken instanceToken =
                result.PossibleInstanceTokens.FirstOrDefault(token => string.Equals((string)token.Value, (string)data));
            result.PresentableInstanceTokens = instanceToken == null ? new InstanceToken[0] : new[] { instanceToken };
            return result;
        }

        #endregion
    }
    public class MarketInfoesPropertyInstanceResolver: IPropertyInstanceResolver
    {
        #region Implementation of IPropertyInstanceResolver

        public InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property)
        {
            MarketInfo[] marketInfos = (MarketInfo[]) data;
            if(marketInfos == null)
                marketInfos = new MarketInfo[0];
            Type[] possibleTypes =
                TypeHelper.GetAllTypes(AllTypeCategory.Game)
                    .Where(type => typeof (MarketInfo).IsAssignableFrom(type) && !type.IsAbstract)
                    .Where(type => marketInfos.Select(info => info.GetType()).All(type1 => type1 != type))
                    .ToArray();
            InstanceResolverResult result = new InstanceResolverResult
            {
                PossibleInstanceTokens = possibleTypes.Select(type => new InstanceToken(type.Name,Activator.CreateInstance(type))).ToArray(),
                PresentableInstanceTokens = marketInfos.Select(info => new InstanceToken(info.GetType().Name, info)).ToArray(),
                IsEditable = true,
                UseValueTypeForEdit = true
            };
            return result;
        }

        #endregion
    }
}