using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RulesBot.Data;
using System;
using YAUL.Extensions;
using YAUL.Utilities;

namespace RulesBot
{
    public static class DIHost
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Setup()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            //ConfigureDbContext(services);

            ServiceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<DiscordBot>();
        }

        private static void ConfigureDbContext(IServiceCollection services)
        {
            string connString = EnvUtils.VariableOrThrow(Constants.Environment.SqlConnectionString);
            services.AddDbContext<RulesBotContext>(opt => opt.UseSqlite(connString));
        }

        public static T Get<T>(bool required = true)
        {
            ServiceProvider.MustNotBeNull(nameof(ServiceProvider));

            if (required) return ServiceProvider.GetRequiredService<T>();
            else return ServiceProvider.GetService<T>();
        }
    }
}
