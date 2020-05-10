

namespace MessageBus
{
    using RabbitMQ.Client;

    /// <summary>
    /// Defines an interface for IMessageBus
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Create connection
        /// </summary>
        IConnection CreateConnection();

        /// <summary>
        /// Method to declare exchange and queue
        /// </summary>
        /// <param name="queueConfig">Queue config</param>
        /// <param name="channel">Channel</param>
        void DeclareExchangeAndQueue(QueueConfiguration queueConfig, IModel channel);

    }
}
