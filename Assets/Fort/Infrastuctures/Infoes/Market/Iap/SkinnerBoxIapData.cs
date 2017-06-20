using System;
using System.Linq;
using System.Reflection;
using Fort.Inspector;

namespace Fort.Info.Market.Iap
{
    public class SkinnerBoxIapData
    {
        public SkinnerBoxIapData()
        {
            SkinnerBoxes = new SkinnerBoxIap[0];
        }
        public SkinnerBoxIap[] SkinnerBoxes { get; set; }
    }
}