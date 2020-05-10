

namespace MessageBus
{
    using System;

    /// <summary>
    /// Defines an interface for IPublisher
    /// </summary>
    public interface IPublisher : IDisposable
    {
        /// <summary>
        /// Method to send message given message properties
        /// </summary>
        /// <param name="queueConfiguration">Queue configuration object</param>
        /// <param name="messageDetail">Message detail object</param>
        void SendMessage(QueueConfigurationBase queueConfiguration, MessageDetailBase messageDetail);
    }
}
