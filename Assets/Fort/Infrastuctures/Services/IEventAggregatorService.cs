using Fort.Aggregator;

namespace Fort
{
    /// <summary>
    /// This service used for aggregating event
    /// </summary>
    public interface IEventAggregatorService
    {
        /// <summary>
        /// Return event
        /// </summary>
        /// <typeparam name="TEventType">Event class</typeparam>
        /// <returns>Return event</returns>
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
    }
}
