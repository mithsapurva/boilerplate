

namespace Common
{
    using Microsoft.Extensions.Hosting;
    using Serilog;

    /// <summary>
    /// Defines the class for LoggingExtensions
    /// </summary>
    public static class LoggingExtensions
    {
        public static IHostBuilder UseLogging(this IHostBuilder hostBuilder, string applicationName = "")
        {
            hostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                var appOptions = context.Configuration.GetOptions<ApplicationOptions>("Application");
                var loggingOptions = context.Configuration.GetOptions<LoggingOptions>("Logging");

                applicationName = string.IsNullOrWhiteSpace(applicationName) ? appOptions.Name : applicationName;

                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration, "Logging")
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("ApplicationName", applicationName);

                if (loggingOptions.ConsoleEnabled)
                {
                    loggerConfiguration.WriteTo
                        .Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {Properties:j}] {Message:lj}{NewLine}{Exception}");
                }
                if (loggingOptions.Seq.Enabled)
                {
                    loggerConfiguration.WriteTo.Seq(loggingOptions.Seq.Url, apiKey: loggingOptions.Seq.ApiKey);
                }
            });
            return hostBuilder;
        }
    }
}
