using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Aggregator;
using Fort.Info.Language;

namespace Assets.Fort.Infrastuctures.Events
{
    public class LanguageActivatedEvent:PubSubEvent<LanguageActivatedEventArgs>
    {
    }

    public class LanguageActivatedEventArgs : EventArgs
    {
        public LanguageInfo Language { get; private set; }

        public LanguageActivatedEventArgs(LanguageInfo language)
        {
            Language = language;
        }
    }
}
