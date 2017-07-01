using System;
using System.Linq;
using System.Reflection;
using Fort.Info.Analytics;
using Fort.Info.Invitation;
using Fort.Info.Language;
using Fort.Info.Market;
using Fort.Info.Market.Iap;
using Fort.Info.PurchasableItem;
using Fort.Inspector;
using Fort.ServerConnection;

namespace Fort.Info
{
    [Info(typeof(FortInfoScriptable),"Fort",false)]
    public class FortInfo: IInfo
    {
        public FortInfo()
        {
            ValueDefenitions = new string[0];
            MarketInfos = new MarketInfo[0];
            Package = new IapPackage();
            InvitationInfo = new InvitationInfo();
            Achievement = new Achievement.Achievement();
            Purchase = new Purchase();
            GameLevel = new GameLevel.GameLevel();
            Analytic = new Analytic();
            Advertisement = new Advertisement.Advertisement();
            SkinnerBox = new SkinnerBox.SkinnerBox();
            Language = new FortLanguage();
            StartupBalance = new Balance();
        }

        public IServerConnectionProvider ServerConnectionProvider { get; set; }
        public FortLanguage Language { get; set; }

        [Inspector(Presentation = "Fort.CustomEditor.ValueDefenitionsPresenter")]

        public string[] ValueDefenitions { get; set; }

        public Balance StartupBalance { get; set; }

        [PresentationTitle("Markets")]
        [PropertyInstanceResolve(typeof(MarketInfoesPropertyInstanceResolver))]
        public MarketInfo[] MarketInfos { get; set; }
        public IapPackage Package { get; set; }
        [PropertyInstanceResolve(typeof(ActiveMarketPropertyInstanceResolver))]
        public string ActiveMarket { get; set; }
        [PresentationTitle("Invitation")]
        public InvitationInfo InvitationInfo { get; set; }
        public Achievement.Achievement Achievement { get; set; }
        public Purchase Purchase { get; set; }
        public GameLevel.GameLevel GameLevel { get; set; }
        public Analytic Analytic { get; set; }
        public Advertisement.Advertisement Advertisement { get; set; }
        public SkinnerBox.SkinnerBox SkinnerBox { get; set; }
        //public StoragePolicy StoragePolicy { get; set; }
        public static FortInfo Instance {get { return InfoResolver.Resolve<FortInfo>(); } }
    }
    public class ActiveMarketPropertyInstanceResolver : IPropertyInstanceResolver
    {
        #region Implementation of IPropertyInstanceResolver

        public InstanceResolverResult ResolvePossibleData(object baseObject, object data, PropertyInfo property)
        {
            InstanceResolverResult result = new InstanceResolverResult
            {
                PossibleInstanceTokens = InfoResolver.Resolve<FortInfo>().MarketInfos.Select(info => new InstanceToken(info.MarketName, info.MarketName)).ToArray()

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

    /*public enum StoragePolicy
    {
        File,
        PlayerPrefs
    }*/
}