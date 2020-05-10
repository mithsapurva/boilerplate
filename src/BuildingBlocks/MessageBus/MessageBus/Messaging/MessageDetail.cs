

namespace MessageBus
{
    using System;

    /// <summary>
    /// Defines the class for MessageDetail
    /// </summary>
    public class MessageDetail : MessageDetailBase
    {
        /// <summary>
        /// Gets or sets Retry count
        /// </summary>
        public int RetryCount { get; set; } = -1;

        /// <summary>
        /// Gets or sets on change event given retry count
        /// </summary>
        public Action<byte[], int> OnChangeEventWithRetry { get; set; }

        /// <summary>
        /// Gets or sets on acknowledge event
        /// </summary>
        public Action OnAckEvent { get; set; }
    }
}
