using Discord;
using Discord.WebSocket;
using marvin2.discord.Services;
using marvin2.Models;
using marvin2.Services;

namespace marvin2.discord.SlashCommands
{
    /// <summary>
    /// Handles the /shoo slash command for the raccoon game.
    /// Allows players to attempt to scare away a raccoon when a game is active in the channel.
    /// Awards points to the first player to successfully shoo the raccoon.
    /// </summary>
    public class ShooCommandHandler : ISlashCommandHandler
    {
        private readonly RaccoonGameService _gameService;
        private readonly ChoreService _choreService;
        private readonly ScoreService _scoreService;
        private readonly ResponseService _responseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShooCommandHandler"/> class.
        /// </summary>
        /// <param name="gameService">Service for managing raccoon game state.</param>
        /// <param name="choreService">Service for accessing Person and chore data from the database.</param>
        /// <param name="scoreService">Service for incrementing player scores.</param>
        /// <param name="responseService">Service for generating random acknowledgement responses.</param>
        public ShooCommandHandler(
            RaccoonGameService gameService,
            ChoreService choreService,
            ScoreService scoreService,
            ResponseService responseService)
        {
            _gameService = gameService;
            _choreService = choreService;
            _scoreService = scoreService;
            _responseService = responseService;
        }

        /// <summary>
        /// Creates the slash command builder for the "shoo" command.
        /// </summary>
        /// <returns>A configured <see cref="SlashCommandBuilder"/> ready to be registered with Discord.</returns>
        public SlashCommandBuilder CreateBuilder()
        {
            SlashCommandBuilder builder = new SlashCommandBuilder();
            builder.WithName("shoo");
            builder.WithDescription("Shoo the raccoon away for a point!");

            return builder;
        }

        /// <summary>
        /// Handles the /shoo command when invoked by a user.
        /// Immediately responds with an acknowledgement to avoid Discord's 3-second interaction timeout,
        /// then checks if a game is active and processes the shoo attempt in the background.
        /// </summary>
        /// <param name="command">The incoming slash command payload.</param>
        /// <param name="responseChannel">The channel where responses should be sent.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task HandleCommand(SocketSlashCommand command, SocketTextChannel responseChannel)
        {
            await command.RespondAsync(_responseService.GetRandomResponse());

            _ = processShoo(command, responseChannel);
        }

        /// <summary>
        /// The shoo command cannot be triggered by a timer. This method does nothing.
        /// </summary>
        /// <param name="responseChannel">The channel where responses would be sent (not used).</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        public Task TriggerResponse(SocketTextChannel responseChannel)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Internal implementation that processes the shoo attempt.
        /// Checks if a game is active, determines the winner, updates the score, and notifies the channel.
        /// </summary>
        /// <param name="command">The incoming slash command payload.</param>
        /// <param name="responseChannel">The channel where the game is active.</param>
        private async Task processShoo(SocketSlashCommand command, SocketTextChannel responseChannel)
        {
            var game = _gameService.GetActiveGame(responseChannel.Id);

            if (game == null)
            {
                await responseChannel.SendMessageAsync("No active raccoon game in this channel. Use a command that starts a game first!");
                return;
            }

            if (game.Winner != null)
            {
                await responseChannel.SendMessageAsync($"The raccoon has already been scared away by {game.Winner.Username}!");
                return;
            }

            var user = command.User;
            var person = _choreService.GetPerson(user.Username);

            if (person == null)
            {
                await responseChannel.SendMessageAsync($"Your Discord username '{user.Username}' is not associated with a person in the system. Please contact an admin.");
                return;
            }

            var updatedScore = _scoreService.IncrementScore(person.Id);

            _gameService.CompleteGame(responseChannel.Id, user);

            var winnerEmbed = RaccoonGameMessageBuilder.BuildWinnerMessage(user, updatedScore);
            var message = await responseChannel.GetMessageAsync(game.MessageId);

            if (message is IUserMessage userMessage)
            {
                await userMessage.ModifyAsync(msg => msg.Embed = winnerEmbed);
            }
        }
    }
}
