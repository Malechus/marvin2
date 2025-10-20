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
    /// <summary>
    /// Configures and boots the Discord bot application.
    /// Responsible for loading configuration, registering services into the DI container,
    /// and starting the bot connection loop.
    /// </summary>
    public class Startup
    {
        private readonly IConfigurationRoot _config;

        /// <summary>
        /// Environment name used to pick the environment-specific configuration file.
        /// Populated from the ASPNETCORE_ENVIRONMENT environment variable, defaults to "Development".
        /// </summary>
        private readonly string _env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        /// <summary>
        /// Initializes a new instance of <see cref="Startup"/> and loads configuration files.
        /// </summary>
        /// <param name="args">Command-line arguments (not currently used).</param>
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
        
        /// <summary>
        /// Convenience entry used by Program.cs to construct and run the startup sequence.
        /// </summary>
        /// <param name="args">Command-line arguments forwarded to the constructor.</param>
        public static async Task RunAsync(string[] args)
        {
            var startup = new Startup(args);
            await startup.RunAsync();
        }
        
        /// <summary>
        /// Builds the DI container, resolves essential services, and starts the bot connection loop.
        /// This method blocks indefinitely once the connection is started.
        /// </summary>
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
        
        /// <summary>
        /// Registers all required services into the provided <see cref="IServiceCollection"/>.
        /// Adds configuration, Discord client and command services, application services, and singletons.
        /// </summary>
        /// <param name="services">The service collection to populate.</param>
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
            .AddSingleton<LoggerService>()
            .AddSingleton<Random>()
            .AddSingleton<ResponseService>()            
            .AddSingleton<StartupService>();
        }
    }
}