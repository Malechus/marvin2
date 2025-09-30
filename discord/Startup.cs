using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using marvin2.Models;
using marvin2.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using marvin2.discord.Services;

namespace marvin2
{
    public class Startup
    {
        private readonly IConfigurationRoot _config;

        private readonly string _env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        public Startup(string[] args)
        {
            if(_env is null)
            {
                _env = "Development";
            }

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{_env}.json");

            _config = builder.Build();
        }
        
        public static async Task RunAsync(string[] args)
        {
            var startup = new Startup(args);
            await startup.RunAsync();
        }
        
        public async Task RunAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<LoggerService>();
            provider.GetRequiredService<CommandHandler>();

            await provider.GetRequiredService<StartupService>().StartConnectionAsync();
            await Task.Delay(-1);
        }
        
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config)
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async
            }))
            .AddSingleton<CommandHandler>()
            .AddSingleton<ChoreService, ChoreService>()
            .AddSingleton<StartupService>()
            .AddSingleton<LoggerService>()
            .AddSingleton<Random>();
        }
    }
}