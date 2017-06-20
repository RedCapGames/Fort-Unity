using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Advertisement;

namespace Fort.Info.Advertisement
{
    public class Advertisement
    {
        public Advertisement()
        {
            AdvertisementProviders = new AdvertisementPriority[0];
        }
        public AdvertisementPriority[] AdvertisementProviders { get; set; }
    }

    public class AdvertisementPriority
    {
        public AdvertisementPriority()
        {
            VideoPriority = -1;
            WallPriority = -1;
            BannerPriority = -1;
        }
        public int VideoPriority { get; set; }
        public int WallPriority { get; set; }
        public int BannerPriority { get; set; }
        public IAdvertisementProvider AdvertisementProvider { get; set; }
    }
}
