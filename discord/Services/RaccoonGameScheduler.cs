using System.Timers;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace marvin2.discord.Services
{
    /// <summary>
    /// Schedules and triggers raccoon games at random intervals (2-4 hours).
    /// Posts game messages to random public Discord channels where @everyone can see them.
    /// Manages game lifecycle including auto-deletion after 5 minutes if no winner.
    /// </summary>
    public class RaccoonGameScheduler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly RaccoonGameService _gameService;
        private readonly ILogger<RaccoonGameScheduler> _logger;
        private System.Timers.Timer _timer;
        private readonly Random _random = new Random();
        private readonly int _minIntervalMs = 7200000; // 2 hours
        private readonly int _maxIntervalMs = 14400000; // 4 hours
        private readonly int _messageDeleteDelayMs = 300000; // 5 minutes

        /// <summary>
        /// Initializes a new instance of <see cref="RaccoonGameScheduler"/>.
        /// </summary>
        /// <param name="discordClient">Discord socket client for accessing channels and guilds.</param>
        /// <param name="gameService">Service for tracking active raccoon games.</param>
        /// <param name="logger">Logger for recording game triggers and errors.</param>
        public RaccoonGameScheduler(
            DiscordSocketClient discordClient,
            RaccoonGameService gameService,
            ILogger<RaccoonGameScheduler> logger)
        {
            _discordClient = discordClient;
            _gameService = gameService;
            _logger = logger;
        }

        /// <summary>
        /// Starts the raccoon game scheduler with a random initial interval.
        /// The scheduler will trigger games every 2-4 hours in random public channels.
        /// This should be called once during bot startup.
        /// </summary>
        public void StartAsync()
        {
            _logger.LogInformation("RaccoonGameScheduler: Starting scheduler");
            scheduleNextGame();
        }

        /// <summary>
        /// Stops the raccoon game scheduler gracefully.
        /// Cleans up the timer and prevents future games from being scheduled.
        /// </summary>
        public void StopAsync()
        {
            _logger.LogInformation("RaccoonGameScheduler: Stopping scheduler");
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Schedules the next game with a random interval between 2-4 hours.
        /// Initializes a new timer and sets up the elapsed event handler.
        /// </summary>
        private void scheduleNextGame()
        {
            int nextIntervalMs = getNextRandomInterval();
            _logger.LogInformation($"RaccoonGameScheduler: Next game scheduled in {nextIntervalMs / 1000 / 60} minutes");

            _timer = new System.Timers.Timer(nextIntervalMs);
            _timer.Elapsed += onTimerElapsed;
            _timer.AutoReset = false;
            _timer.Start();
        }

        /// <summary>
        /// Gets the next random interval between 2-4 hours in milliseconds.
        /// </summary>
        /// <returns>A random interval in milliseconds.</returns>
        private int getNextRandomInterval()
        {
            return _random.Next(_minIntervalMs, _maxIntervalMs + 1);
        }

        /// <summary>
        /// Handles the timer elapsed event to trigger a new raccoon game.
        /// Selects a random public channel, posts the game message, and schedules deletion.
        /// Reschedules the timer for the next game.
        /// </summary>
        private void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _ = triggerGameAsync();
        }

        /// <summary>
        /// Asynchronously triggers a new raccoon game.
        /// Posts the game message to a random public channel and sets up auto-deletion.
        /// </summary>
        private async Task triggerGameAsync()
        {
            try
            {
                var channel = selectRandomPublicChannel();

                if (channel == null)
                {
                    _logger.LogWarning("RaccoonGameScheduler: No valid public channels found for game trigger");
                    scheduleNextGame();
                    return;
                }

                _logger.LogInformation($"RaccoonGameScheduler: Triggering game in channel {channel.Name} ({channel.Id})");

                var embed = RaccoonGameMessageBuilder.BuildGameMessage();
                var sentMessage = await channel.SendMessageAsync(embed: embed);

                _gameService.StartGame(channel.Id, sentMessage.Id);

                scheduleMessageDeletion(channel, sentMessage.Id);
                scheduleNextGame();
            }
            catch (Exception ex)
            {
                _logger.LogError($"RaccoonGameScheduler: Error triggering game - {ex.Message}");
                scheduleNextGame();
            }
        }

        /// <summary>
        /// Schedules a game message for automatic deletion after 5 minutes if no winner.
        /// </summary>
        /// <param name="channel">The channel containing the message.</param>
        /// <param name="messageId">The ID of the message to delete.</param>
        private void scheduleMessageDeletion(SocketTextChannel channel, ulong messageId)
        {
            _ = deleteMessageAfterDelayAsync(channel, messageId);
        }

        /// <summary>
        /// Asynchronously deletes a message after a delay, if the game hasn't been won.
        /// </summary>
        /// <param name="channel">The channel containing the message.</param>
        /// <param name="messageId">The ID of the message to delete.</param>
        private async Task deleteMessageAfterDelayAsync(SocketTextChannel channel, ulong messageId)
        {
            try
            {
                await Task.Delay(_messageDeleteDelayMs);

                var game = _gameService.GetActiveGame(channel.Id);

                if (game != null && game.MessageId == messageId && game.Winner == null)
                {
                    var message = await channel.GetMessageAsync(messageId);
                    if (message is IUserMessage userMessage)
                    {
                        await userMessage.DeleteAsync();
                        _logger.LogInformation($"RaccoonGameScheduler: Auto-deleted game message in channel {channel.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"RaccoonGameScheduler: Error deleting message - {ex.Message}");
            }
        }

        /// <summary>
        /// Selects a random text channel where @everyone can read messages.
        /// Filters channels to only include those with public visibility.
        /// </summary>
        /// <returns>A random public text channel, or null if none are available.</returns>
        private SocketTextChannel selectRandomPublicChannel()
        {
            var publicChannels = getAllPublicChannels();

            if (publicChannels.Count == 0)
            {
                return null;
            }

            int randomIndex = _random.Next(publicChannels.Count);
            return publicChannels[randomIndex];
        }

        /// <summary>
        /// Gets all text channels where @everyone has permission to read messages.
        /// Queries all guilds the bot is connected to.
        /// </summary>
        /// <returns>A list of public text channels.</returns>
        private List<SocketTextChannel> getAllPublicChannels()
        {
            var publicChannels = new List<SocketTextChannel>();

            foreach (var guild in _discordClient.Guilds)
            {
                foreach (var channel in guild.TextChannels)
                {
                    if (isChannelPublic(channel))
                    {
                        publicChannels.Add(channel);
                    }
                }
            }

            return publicChannels;
        }

        /// <summary>
        /// Checks if a text channel is publicly readable by @everyone.
        /// A channel is public if the @everyone role has explicit or implicit read permissions.
        /// </summary>
        /// <param name="channel">The channel to check.</param>
        /// <returns>True if the channel is public, false otherwise.</returns>
        private bool isChannelPublic(SocketTextChannel channel)
        {
            var everyoneRole = channel.Guild.EveryoneRole;
            var permissions = channel.GetPermissionOverwrite(everyoneRole);

            if (permissions == null)
            {
                // No explicit override, so @everyone has default permissions (which includes ViewChannel)
                return true;
            }

            return permissions.Value.ViewChannel == PermValue.Allow;
        }
    }
}
