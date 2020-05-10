

namespace MessageBus
{
    /// <summary>
    /// Defines an enum for RabbitMQExchangeType
    /// </summary>
    public enum RabbitMQExchangeType
    {
        /// <summary>
        /// Topic exchange
        /// </summary>
        topic = 0,

        /// <summary>
        /// Fanout exchange
        /// </summary>
        fanout,

        /// <summary>
        /// Direct exchange
        /// </summary>
        direct,

        /// <summary>
        /// Headers exchange
        /// </summary>
        headers
    }
}
