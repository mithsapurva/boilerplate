
namespace MessageBus
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System;
    using System.Globalization;

    /// <summary>
    /// Defines a class for RabbitMQSubscriber
    /// </summary>
    public class RabbitMQSubscriber : ISubscriber
    {
        /// <summary>
        /// Define private member for channel
        /// </summary>
        private IModel channel;

        /// <summary>
        /// Define private member for connection
        /// </summary>
        private IConnection connection;

        /// <summary>
        /// Constructor for RabbitMQ subscirber
        /// </summary>
        /// <param name="queueConfig">Queue configuration</param>
        public RabbitMQSubscriber(QueueConfiguration queueConfig)
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
        /// Method to consume message
        /// </summary>
        /// <param name="queueConfig">Queue configuration</param>
        /// <param name="messageDetail">Message detail</param>
        public void Consume(QueueConfigurationBase queueConfiguration, MessageDetailBase messageDetail)
        {
            QueueConfiguration configuration = (QueueConfiguration)queueConfiguration;
            MessageDetail detail = (MessageDetail)messageDetail;

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, args) =>
            {
                try
                {
                    byte[] body = args.Body;
                    var count = 0;
                    if (args.BasicProperties != null && args.BasicProperties.Headers != null)
                    {
                        if (args.BasicProperties.Headers["RetryCount"] != null
                                            && Convert.ToInt32(args.BasicProperties.Headers["RetryCount"], CultureInfo.InvariantCulture) > -1)
                        {
                            var retryCount = args.BasicProperties.Headers["RetryCount"];
                            count = Convert.ToInt32(retryCount, CultureInfo.InvariantCulture);
                            detail.OnChangeEventWithRetry(body, count);
                        }
                        else
                        {
                            detail.OnChangeEvent(body);
                        }
                    }
                    channel.BasicAck(args.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    channel.BasicAck(args.DeliveryTag, false);
                }
            };
            bool autoAck = false;
            channel.BasicConsume(configuration.QueueName, autoAck, consumer);
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposeManagedResources">Whether to dispose managed resources</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                if (channel != null && channel.IsOpen)
                {
                    channel.Close();
                }

                if (connection != null && connection.IsOpen)
                {
                    connection.Close();
                }
            }
        }
    }
}
