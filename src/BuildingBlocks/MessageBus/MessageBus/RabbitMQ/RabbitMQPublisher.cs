

namespace MessageBus
{
    using RabbitMQ.Client;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a class for RabbitMQPublisher
    /// </summary>
    public class RabbitMQPublisher : IPublisher
    {
        /// <summary>
        /// Define private member for channel
        /// </summary>
        private static IModel channel;

        /// <summary>
        /// Define private member for connection
        /// </summary>
        private static IConnection connection;

        /// <summary>
        /// Constructor for rabbitmq publisher
        /// </summary>
        /// <param name="queueConfig">Queue configuration</param>
        public RabbitMQPublisher(QueueConfiguration queueConfig)
        {
            if (queueConfig != null && !string.IsNullOrEmpty(queueConfig.QueueName))
            {
                IMessageBus messageBus = new RabbitMQMessageBus();
                connection = messageBus.CreateConnection();
                channel = connection.CreateModel();
                messageBus.DeclareExchangeAndQueue(queueConfig, channel);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Method to dispose unmanaged resources
        /// </summary>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                if (connection != null && connection.IsOpen)
                {
                    connection.Close();
                }

                if (channel != null && channel.IsOpen)
                {
                    channel.Close();
                }
            }
        }

        /// <summary>
        /// Method to send message
        /// </summary>
        /// <param name="queueConfig">Queue configuration</param>
        /// <param name="messageDetail">Mesage detail</param>
        public void SendMessage(QueueConfigurationBase queueConfiguration, MessageDetailBase messageDetail)
        {
            if (queueConfiguration != null && messageDetail != null && !string.IsNullOrEmpty(queueConfiguration.QueueName))
            {
                QueueConfiguration configuration = (QueueConfiguration)queueConfiguration;
                MessageDetail detail = messageDetail as MessageDetail;

                IBasicProperties basicProperties = channel.CreateBasicProperties();
                basicProperties.Persistent = true;
                basicProperties.Headers = new Dictionary<string, object>();
                basicProperties.Headers.Add("RetryCount", detail.RetryCount);               
                channel.BasicPublish(configuration.ExchangeName, configuration.RoutingKey, basicProperties, detail.MessageContent);
            }
        }
    }
}
