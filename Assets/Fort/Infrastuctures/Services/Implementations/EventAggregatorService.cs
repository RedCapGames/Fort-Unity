using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort.Aggregator;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(IEventAggregatorService))]
    public class EventAggregatorService:MonoBehaviour,IEventAggregatorService
    {
        EventAggregator _eventAggregator = new EventAggregator();
        #region Implementation of IEventAggregatorService

        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            return _eventAggregator.GetEvent<TEventType>();
        }

        #endregion
    }
}
