using System;
using Fort.Aggregator;

namespace Assets.Fort.Infrastuctures.Events
{
    public class RemoveAdRequestedEvent:PubSubEvent<RemoveAdRequestedEventArgs>
    {
    }

    public class RemoveAdRequestedEventArgs : EventArgs
    {
        public bool StandardBanner { get; private set; }

        public RemoveAdRequestedEventArgs(bool standardBanner)
        {
            StandardBanner = standardBanner;
        }
    }
}
