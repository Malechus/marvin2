using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using marvin2.discord.SlashCommands;
using marvin2.Services;

namespace marvin2.discord.Services
{
    public class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly ChoreService _choreservice;
        private readonly ResponseService _responseservice;
        private readonly ListChores _listChores;
        
        public StartupService(
            IServiceProvider serviceProvider,
            DiscordSocketClient discordSocketClient,
            CommandService commandService,
            IConfigurationRoot configurationRoot,
            ChoreService choreService,
            ResponseService responseService
        )
        {
            _provider = serviceProvider;
            _client = discordSocketClient;
            _commands = commandService;
            _config = configurationRoot;
            _choreservice = choreService;
            _responseservice = responseService;
            _listChores = new ListChores(_choreservice, _responseservice);
        }
        
        public async Task StartConnectionAsync()
        {
            _client.SlashCommandExecuted += SlashCommand_Executed;
            _client.Ready += Client_Ready;
            _client.Ready += Announce;

            await _client.LoginAsync(TokenType.Bot, _config["Discord:Token"]);
            await _client.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
        
        private async Task Announce()
        {
            ISocketMessageChannel channel = await _client.GetChannelAsync(ulong.Parse(_config["Discord:Channels:Announce"])) as ISocketMessageChannel;

            await channel.SendMessageAsync(_responseservice.GetRandomGreeting());
            //TODO add self test
        }
        
        public async Task Client_Ready()
        {
            SocketGuild server = _client.GetGuild(ulong.Parse(_config["Discord:ServerID"]));
            
            SlashCommandBuilder listBuilder = _listChores.CreateBuilder();
            server.CreateApplicationCommandAsync(listBuilder.Build());
        }

        public async Task SlashCommand_Executed(SocketSlashCommand command)
        {
            switch(command.Data.Name)
            {
                case "listchores":
                    SocketGuild guild = _client.GetGuild(ulong.Parse(_config["Discord:ServerID"]));
                    SocketTextChannel responseChannel = (SocketTextChannel)guild.GetChannel(ulong.Parse(_config["Discord:Channels:Chore_List"]));
                    _listChores.HandleCommand(command, responseChannel);
                    break;
                default:
                    command.RespondAsync("Command unrecognized, try again.");
                    break;
            }
        }
    }
}