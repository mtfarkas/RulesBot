using Microsoft.Extensions.DependencyInjection;
using RulesBot.Core.Extensions;
using System;

namespace RulesBot
{
    public static class DIHost
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Setup()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<DiscordBot>();
        }

        public static T Get<T>(bool required = true)
        {
            ServiceProvider.MustNotBeNull(nameof(ServiceProvider));

            if (required) return ServiceProvider.GetRequiredService<T>();
            else return ServiceProvider.GetService<T>();
        }
    }
}
