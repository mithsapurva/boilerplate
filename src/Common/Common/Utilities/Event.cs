
namespace Common
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///  Supertype for all Event types
    /// </summary>
    public interface IEvent
    {
        Guid Id { get; }
        int EventVersion { get; }
        DateTime OccurredOn { get; }
    }

    /// <summary>
    /// Defines the interface for IIntegrationEvent
    /// </summary>
    public interface IIntegrationEvent : IEvent
    {
    }

    /// <summary>
    /// Defines the interface for IEventHandler
    /// </summary>
    public interface IEventHandler<in TEvent, TResult>
        where TEvent : IEvent
    {
        Task<TResult> Handle(TEvent request, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Defines the interface for IDomainEventDispatcher
    /// </summary>
    public interface IDomainEventDispatcher : IDisposable
    {
        Task Dispatch(IEvent @event);
    }

    /// <summary>
    /// Defines the class for EventBase
    /// </summary>
    public abstract class EventBase : IEvent
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public int EventVersion { get; protected set; } = 1;
        public DateTime OccurredOn { get; protected set; } = DateTimeOffset.Now.UtcDateTime;
    }

    /// <summary>
    /// Defines the class for IntegrationEventBase
    /// </summary>
    public abstract class IntegrationEventBase : EventBase, IIntegrationEvent
    {
    }

    /// <summary>
    /// Defines the class for EventEnvelope
    /// </summary>
    public class EventEnvelope : EventBase
    {
        public EventEnvelope(IEvent @event)
        {
            Event = @event;
        }

        public IEvent Event { get; }
    }

    /// <summary>
    /// Defines the class for NoOpDomainEventDispatcher
    /// </summary>
    public class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public void Dispose()
        {
        }

        public Task Dispatch(IEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
