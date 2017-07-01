using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Aggregator;
using Fort.Info.Market.Iap;

namespace Fort.Events
{
    public class IapPackagePurchasedEvent:PubSubEvent<IapPackagePurchasedEventArgs>
    {
    }

    public class IapPackagePurchasedEventArgs : EventArgs
    {
        public IapPackageInfo IapPackage { get; private set; }

        public IapPackagePurchasedEventArgs(IapPackageInfo iapPackage)
        {
            IapPackage = iapPackage;
        }
    }
}
