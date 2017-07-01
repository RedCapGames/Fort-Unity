



namespace Fort.Aggregator
{
    /// <summary>
    /// Defines an interface to get instances of an event type.
    /// </summary>
    internal interface IEventAggregator
    {
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
    }
}
