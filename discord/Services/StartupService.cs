using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace marvin2.discord.Services
{
    public class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        
        public StartupService(
            IServiceProvider serviceProvider,
            DiscordSocketClient discordSocketClient,
            CommandService commandService,
            IConfigurationRoot configurationRoot
        )
        {
            _provider = serviceProvider;
            _client = discordSocketClient;
            _commands = commandService;
            _config = configurationRoot;
        }
        
        public async Task StartConnectionAsync()
        {
            _client.Ready += Announce;

            await _client.LoginAsync(TokenType.Bot, _config["Discord:Token"]);
            await _client.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
        
        private async Task Announce()
        {
            ISocketMessageChannel channel = await _client.GetChannelAsync(ulong.Parse(_config["Discord:Channels:Announce"])) as ISocketMessageChannel;

            await channel.SendMessageAsync("I'm awake.");
            //TODO add self test
        }
    }
}