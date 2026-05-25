using Discord;
using Discord.WebSocket;
using marvin2.discord.Services;
using marvin2.Models;
using marvin2.Services;

namespace marvin2.discord.SlashCommands
{
    /// <summary>
    /// Handles the /hunt slash command for the raccoon game.
    /// Allows players to attempt to catch a raccoon when a game is active in the channel.
    /// Awards points to the first player to successfully hunt the raccoon.
    /// </summary>
    public class HuntCommandHandler : ISlashCommandHandler
    {
        private readonly RaccoonGameService _gameService;
        private readonly ChoreService _choreService;
        private readonly ScoreService _scoreService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HuntCommandHandler"/> class.
        /// </summary>
        /// <param name="gameService">Service for managing raccoon game state.</param>
        /// <param name="choreService">Service for accessing Person and chore data from the database.</param>
        /// <param name="scoreService">Service for incrementing player scores.</param>
        public HuntCommandHandler(
            RaccoonGameService gameService,
            ChoreService choreService,
            ScoreService scoreService)
        {
            _gameService = gameService;
            _choreService = choreService;
            _scoreService = scoreService;
        }

        /// <summary>
        /// Creates the slash command builder for the "hunt" command.
        /// </summary>
        /// <returns>A configured <see cref="SlashCommandBuilder"/> ready to be registered with Discord.</returns>
        public SlashCommandBuilder CreateBuilder()
        {
            SlashCommandBuilder builder = new SlashCommandBuilder();
            builder.WithName("hunt");
            builder.WithDescription("Hunt the raccoon and catch it for a point!");

            return builder;
        }

        /// <summary>
        /// Handles the /hunt command when invoked by a user.
        /// Checks if a game is active and processes the hunt attempt in the background.
        /// The immediate acknowledgement is already handled by StartupService.
        /// </summary>
        /// <param name="command">The incoming slash command payload.</param>
        /// <param name="responseChannel">The channel where responses should be sent.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task HandleCommand(SocketSlashCommand command, SocketTextChannel responseChannel)
        {
            _ = processHunt(command, responseChannel);
        }

        /// <summary>
        /// The hunt command cannot be triggered by a timer. This method does nothing.
        /// </summary>
        /// <param name="responseChannel">The channel where responses would be sent (not used).</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        public Task TriggerResponse(SocketTextChannel responseChannel)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Internal implementation that processes the hunt attempt.
        /// Checks if a game is active, determines the winner, updates the score, and notifies the channel.
        /// </summary>
        /// <param name="command">The incoming slash command payload.</param>
        /// <param name="responseChannel">The channel where the game is active.</param>
        private async Task processHunt(SocketSlashCommand command, SocketTextChannel responseChannel)
        {
            var game = _gameService.GetActiveGame(responseChannel.Id);

            if (game == null)
            {
                await responseChannel.SendMessageAsync("No active raccoon game in this channel. Use a command that starts a game first!");
                return;
            }

            if (game.Winner != null)
            {
                await responseChannel.SendMessageAsync($"The raccoon has already been caught by {game.Winner.Username}!");
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
