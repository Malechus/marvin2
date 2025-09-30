using Azure.Messaging;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace marvin2.discord.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;
        
        public CommandHandler(
            DiscordSocketClient discordSocketClient,
            CommandService commandService,
            IServiceProvider serviceProvider
        )
        {
            _client = discordSocketClient;
            _commands = commandService;
            _provider = serviceProvider;

            //_client.MessageReceived += OnMessageReceivedAsync;
        }
        
        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            SocketUserMessage? message = s as SocketUserMessage;
            if (message is null) return;
            if (message.Author.Id == _client.CurrentUser.Id) return;

            SocketCommandContext context = new SocketCommandContext(_client, message);

            int argPos = 0;
            if(message.HasCharPrefix('$', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                IResult result = await _commands.ExecuteAsync(context, argPos, _provider);
                
                if(!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync("There was an error, please try again.");
                    //TODO: Add error handling.
                }
            }
        }
    }
}