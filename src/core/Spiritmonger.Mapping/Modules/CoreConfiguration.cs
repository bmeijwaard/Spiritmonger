using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spiritmonger.Cmon.Settings;

namespace Spiritmonger.Mapping.Modules
{
    public static class CoreConfiguration
    {
        public static IServiceCollection Load(IServiceCollection services, IConfiguration config)
        {
            services.AddOptions();
            services.Configure<ConnectionStrings>(opt => config.GetSection("ConnectionStrings").Bind(opt));

            return services;
        }
    }
}
