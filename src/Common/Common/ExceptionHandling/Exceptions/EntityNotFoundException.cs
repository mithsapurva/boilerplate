

namespace Common
{
    using System;

    /// <summary>
    /// Defines the class for EntityNotFoundException
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        public string message { get; set; }

        public override string Message
        {
            get { return this.message; }
        }

        public EntityNotFoundException(string message)
        {
            this.message = message;
        }
    }
}
