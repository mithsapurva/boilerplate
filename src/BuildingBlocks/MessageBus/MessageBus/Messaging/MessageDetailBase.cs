

namespace MessageBus
{
    using System;

    /// <summary>
    /// Defines the class for MessageDetailBase
    /// </summary>
    public class MessageDetailBase
    {
        /// <summary>
        /// Gets or sets message content
        /// </summary>
        public byte[] MessageContent { get; set; }

        /// <summary>
        /// Gets or sets on change event
        /// </summary>
        public Action<byte[]> OnChangeEvent { get; set; }
    }
}
