using Fort.Aggregator;

namespace Fort
{
    public interface IEventAggregatorService
    {
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
    }
}
