using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using marvin2.discord.SlashCommands;
using marvin2.Services;
using System.Timers;
using data.Services;
using System.Net.Sockets;

namespace marvin2.discord.Services
{
    /// <summary>
    /// Responsible for initializing and starting Discord-related services.
    /// Registers slash commands, hooks Discord client events, and orchestrates
    /// the bot's startup flow (login, start, and module registration).
    /// </summary>
    public class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly ChoreService _choreservice;
        private readonly ResponseService _responseService;
        private readonly PiService _piService;
        private readonly RaccoonGameScheduler _gameScheduler;
        private readonly ISlashCommandHandler _listChores;
        private readonly ISlashCommandHandler _statusHandler;
        private readonly ISlashCommandHandler _huntHandler;
        private readonly ISlashCommandHandler _shooHandler;
        private System.Timers.Timer _timer;
        
         /// <summary>
         /// Constructs a new instance of <see cref="StartupService"/>.
         /// </summary>
         /// <param name="serviceProvider">DI service provider used to resolve modules and services.</param>
         /// <param name="discordSocketClient">Discord socket client used to interact with the gateway.</param>
         /// <param name="commandService">Command service used to register text command modules.</param>
         /// <param name="configurationRoot">Configuration root for reading settings like tokens and channel IDs.</param>
         /// <param name="choreService">Application service for retrieving chore and person data.</param>
         /// <param name="responseService">Service used to select and format bot responses.</param>
         /// <param name="piService">Service for Pi-hole queries.</param>
         /// <param name="gameService">Service for managing raccoon game state.</param>
         /// <param name="scoreService">Service for incrementing player scores.</param>
         /// <param name="gameScheduler">Service for scheduling and triggering raccoon games.</param>
        public StartupService(
            IServiceProvider serviceProvider,
            DiscordSocketClient discordSocketClient,
            CommandService commandService,
            IConfigurationRoot configurationRoot,
            ChoreService choreService,
            ResponseService responseService,
            PiService piService,
            RaccoonGameService gameService,
            ScoreService scoreService,
            RaccoonGameScheduler gameScheduler
        )
        {
            _provider = serviceProvider;
            _client = discordSocketClient;
            _commands = commandService;
            _config = configurationRoot;
            _choreservice = choreService;
            _responseService = responseService;
            _piService = piService;
            _gameScheduler = gameScheduler;
            _listChores = new ListChores(_choreservice, _responseService);
            _statusHandler = new Status(_piService);
            _huntHandler = new HuntCommandHandler(gameService, choreService, scoreService);
            _shooHandler = new ShooCommandHandler(gameService, choreService, scoreService, responseService);
        }
        
        /// <summary>
        /// Configures event handlers, logs into Discord, starts the client, and registers command modules.
        /// This method should be awaited during application startup.
        /// </summary>
        public async Task StartConnectionAsync()
        {
            _client.SlashCommandExecuted += SlashCommand_Executed;
            _client.Ready += Client_Ready;
            _client.Ready += Timer_Start;
            _client.Ready += GameScheduler_Start;
            _client.Ready += Announce;

            await _client.LoginAsync(TokenType.Bot, _config["Discord:Token"]);
            await _client.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
        
        /// <summary>
        /// Sends a startup announcement message to the configured announce channel.
        /// Uses <see cref="ResponseService.GetRandomGreeting"/> to select the message.
        /// </summary>
        private async Task Announce()
        {
            ISocketMessageChannel channel = await _client.GetChannelAsync(ulong.Parse(_config["Discord:Channels:Announce"])) as ISocketMessageChannel;

            await channel.SendMessageAsync(_responseService.GetRandomGreeting());
            //TODO add self test
        }
        
        /// <summary>
        /// Starts the raccoon game scheduler when the Discord client is ready.
        /// </summary>
        private async Task GameScheduler_Start()
        {
            _gameScheduler.StartAsync();
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// Invoked when the Discord client signals it is ready.
        /// Registers or creates guild-level application commands such as the chore list command.
        /// </summary>
        public async Task Client_Ready()
        {
            SocketGuild server = _client.GetGuild(ulong.Parse(_config["Discord:ServerID"]));
            
            SlashCommandBuilder listBuilder = _listChores.CreateBuilder();
            SlashCommandBuilder statusBuilder = _statusHandler.CreateBuilder();
            SlashCommandBuilder huntBuilder = _huntHandler.CreateBuilder();
            SlashCommandBuilder shooBuilder = _shooHandler.CreateBuilder();
            server.CreateApplicationCommandAsync(listBuilder.Build());
            server.CreateApplicationCommandAsync(statusBuilder.Build());
            server.CreateApplicationCommandAsync(huntBuilder.Build());
            server.CreateApplicationCommandAsync(shooBuilder.Build());
        }

        /// <summary>
        /// Central handler for executed slash commands.
        /// Immediately responds with a short, pre-generated response to avoid interaction timeouts,
        /// then dispatches longer-running operations (like listing chores) to background handlers.
        /// </summary>
        /// <param name="command">The slash command that was executed.</param>
        private async Task SlashCommand_Executed(SocketSlashCommand command)
        {
            //Immediately respond to command here with a random, pregenerated response.
            //"Respnses" later will use the SocketTextChannel to PostAsync instead of "Responding"
            //This eliminates the three second timer on command responses for the bot and allows for longer behind the scenes operations to take place
            //NOTE: Long loading operations should still be put into interactions/context menus 
            //Some use cases for this bot include printing lists of data to specific read only channels, which is why bypassing this rule is elected here
            await command.RespondAsync(_responseService.GetRandomResponse());
            
            SocketGuild guild = _client.GetGuild(ulong.Parse(_config["Discord:ServerID"]));
            SocketTextChannel responseChannel = (SocketTextChannel)command.Channel;
            
            switch(command.Data.Name)
            {
                case "listchores":
                    responseChannel = (SocketTextChannel)guild.GetChannel(ulong.Parse(_config["Discord:Channels:Chore_List"]));
                    _listChores.HandleCommand(command, responseChannel);
                    break;
                case "status":
                    _statusHandler.HandleCommand(command, responseChannel);
                    break;
                case "hunt":
                    _huntHandler.HandleCommand(command, responseChannel);
                    break;
                case "shoo":
                    _shooHandler.HandleCommand(command, responseChannel);
                    break;
                default:
                    command.RespondAsync("Command unrecognized, try again.");
                    break;
            }
        }
        
        private async Task Timer_Start()
        {
            DateTime now = DateTime.Now;
            DateTime executeTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);

            if (now.CompareTo(executeTime) > 0) executeTime = executeTime.AddDays(1);

            double ticks = (executeTime - now).TotalMilliseconds;
            _timer = new System.Timers.Timer(ticks);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }
        
        private void Timer_Elapsed(object Sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            SocketGuild guild = _client.GetGuild(ulong.Parse(_config["Discord:ServerID"]));
            SocketTextChannel responseChannel = (SocketTextChannel)guild.GetChannel(ulong.Parse(_config["Discord:Channels:Chore_List"]));
            _listChores.TriggerResponse(responseChannel);
            Thread.Sleep(120000);
            Timer_Start();
        }
    }
}