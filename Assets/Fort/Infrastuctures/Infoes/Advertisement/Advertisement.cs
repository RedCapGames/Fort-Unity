using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Advertisement;

namespace Fort.Info
{
    public class Advertisement
    {
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
