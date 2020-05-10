

namespace MessageBus
{
    using System;

    /// <summary>
    /// Defines an interface for ISubscriber
    /// </summary>
    public interface ISubscriber : IDisposable
    {
        /// <summary>
        /// Method to subscribe to queue and processing the message
        /// </summary>
        /// <param name="queueConfiguration">Queue configuration object</param>
        /// <param name="messageDetail">Message detail object</param>
        void Consume(QueueConfigurationBase queueConfiguration, MessageDetailBase messageDetail);
    }
}
