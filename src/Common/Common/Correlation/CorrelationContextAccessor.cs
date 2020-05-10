

namespace Common
{
    using CorrelationId;
    using System.Threading;

    /// <summary>
    /// Defines the class for CorrelationContextAccessor
    /// </summary>
    public class CorrelationContextAccessor : ICorrelationContextAccessor
    {
        private static AsyncLocal<CorrelationContext> correlationContext = new AsyncLocal<CorrelationContext>();

        public static ICorrelationContextAccessor Current { get; } = new CorrelationContextAccessor();

        public static string CorrelationId => Current.CorrelationContext.CorrelationId;

        public CorrelationContext CorrelationContext
        {
            get => correlationContext.Value;
            set => correlationContext.Value = value;
        }
    }
}
