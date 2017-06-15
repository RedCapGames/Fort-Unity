using Fort.Info;
using UnityEngine;

namespace Fort.Inspector
{
    public class FortInfo
    {        
#if UNITY_EDITOR
        [Inspector(Presentation = "Fort.CustomEditor.ValueDefenitionsPresenter")]
#endif
        public string[] ValueDefenitions { get; set; }
        public MarketInfo[] MarketInfos { get; set; }
        public string ActiveMarket { get; set; }
        public InvitationInfo InvitationInfo { get; set; }
        public Achievement Achievement { get; set; }
        public Purchase Purchase { get; set; }
        public GameLevel GameLevel { get; set; }
        public Analytic Analytic { get; set; }
        public Info.Advertisement Advertisement { get; set; }
        public SkinnerBox SkinnerBox { get; set; }
    }
}