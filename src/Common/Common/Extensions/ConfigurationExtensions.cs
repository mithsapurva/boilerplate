
namespace Common
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Defines the class for ConfigurationExtensions
    /// </summary>
    public static class ConfigurationExtensions
    {
        public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(section).Bind(model);
            return model;
        }
    }
}
