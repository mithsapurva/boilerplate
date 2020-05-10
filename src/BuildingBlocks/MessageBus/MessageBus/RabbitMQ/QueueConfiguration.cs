

namespace MessageBus
{
    /// <summary>
    /// Defines the class for QueueConfiguration
    /// </summary>
    public class QueueConfiguration : QueueConfigurationBase
    {
        /// <summary>
        /// Gets or sets exchange name
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Gets or sets exchange type
        /// </summary>
        public RabbitMQExchangeType ExchangeType { get; set; } = RabbitMQExchangeType.topic;

        /// <summary>
        /// Gets or sets channel prefetch count
        /// </summary>
        public ushort ChannelPrefetchCount { get; set; } = 100;

        /// <summary>
        /// Gets or sets routing key
        /// </summary>
        public string RoutingKey { get; set; }
    }
}
