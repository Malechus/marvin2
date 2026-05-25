using Discord;

namespace marvin2.discord.Services
{
    /// <summary>
    /// Represents the state of an active raccoon game in a specific Discord channel.
    /// Tracks the channel, message, winner, and start time for a single game instance.
    /// </summary>
    public class RaccoonGame
    {
        /// <summary>
        /// The Discord channel ID where the game is active.
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        /// The Discord message ID of the game prompt message.
        /// </summary>
        public ulong MessageId { get; set; }

        /// <summary>
        /// The user who won the game, or null if no winner yet.
        /// </summary>
        public IUser? Winner { get; set; }

        /// <summary>
        /// The UTC timestamp when the game was started.
        /// </summary>
        public DateTime StartTime { get; set; }
    }

    /// <summary>
    /// Manages the lifecycle of raccoon games across multiple Discord channels.
    /// Provides methods to start games, track active games, and complete them when a winner is determined.
    /// Ensures only one game is active per channel at a time.
    /// </summary>
    public class RaccoonGameService
    {
        private readonly Dictionary<ulong, RaccoonGame> _activeGames;

        /// <summary>
        /// Initializes a new instance of <see cref="RaccoonGameService"/>.
        /// </summary>
        public RaccoonGameService()
        {
            _activeGames = new Dictionary<ulong, RaccoonGame>();
        }

        /// <summary>
        /// Starts a new raccoon game in the specified channel.
        /// If a game is already active in this channel, it is replaced.
        /// </summary>
        /// <param name="channelId">The ID of the channel where the game should start.</param>
        /// <param name="messageId">The ID of the game prompt message.</param>
        /// <returns>The newly created <see cref="RaccoonGame"/> instance.</returns>
        public RaccoonGame StartGame(ulong channelId, ulong messageId)
        {
            return startGame(channelId, messageId);
        }

        /// <summary>
        /// Internal implementation that creates and stores a new game instance.
        /// </summary>
        /// <param name="channelId">The ID of the channel where the game should start.</param>
        /// <param name="messageId">The ID of the game prompt message.</param>
        /// <returns>The newly created <see cref="RaccoonGame"/> instance.</returns>
        private RaccoonGame startGame(ulong channelId, ulong messageId)
        {
            var game = new RaccoonGame
            {
                ChannelId = channelId,
                MessageId = messageId,
                Winner = null,
                StartTime = DateTime.UtcNow
            };

            _activeGames[channelId] = game;
            return game;
        }

        /// <summary>
        /// Retrieves the active game in a specific channel.
        /// </summary>
        /// <param name="channelId">The ID of the channel to check.</param>
        /// <returns>The active <see cref="RaccoonGame"/> if one exists, otherwise null.</returns>
        public RaccoonGame? GetActiveGame(ulong channelId)
        {
            return getActiveGame(channelId);
        }

        /// <summary>
        /// Internal implementation that looks up the game in the cache.
        /// </summary>
        /// <param name="channelId">The ID of the channel to check.</param>
        /// <returns>The active <see cref="RaccoonGame"/> if one exists, otherwise null.</returns>
        private RaccoonGame? getActiveGame(ulong channelId)
        {
            if (_activeGames.TryGetValue(channelId, out var game))
            {
                return game;
            }
            return null;
        }

        /// <summary>
        /// Marks a game as complete by setting the winner and removing it from the active games cache.
        /// </summary>
        /// <param name="channelId">The ID of the channel where the game is active.</param>
        /// <param name="winner">The user who won the game.</param>
        public void CompleteGame(ulong channelId, IUser winner)
        {
            completeGame(channelId, winner);
        }

        /// <summary>
        /// Internal implementation that clears the game state for a channel.
        /// </summary>
        /// <param name="channelId">The ID of the channel where the game is active.</param>
        /// <param name="winner">The user who won the game.</param>
        private void completeGame(ulong channelId, IUser winner)
        {
            if (_activeGames.TryGetValue(channelId, out var game))
            {
                game.Winner = winner;
                _activeGames.Remove(channelId);
            }
        }

        /// <summary>
        /// Checks if there is an active game in the specified channel.
        /// </summary>
        /// <param name="channelId">The ID of the channel to check.</param>
        /// <returns>True if a game is active in this channel, false otherwise.</returns>
        public bool IsGameActive(ulong channelId)
        {
            return isGameActive(channelId);
        }

        /// <summary>
        /// Internal implementation that checks the game cache.
        /// </summary>
        /// <param name="channelId">The ID of the channel to check.</param>
        /// <returns>True if a game is active in this channel, false otherwise.</returns>
        private bool isGameActive(ulong channelId)
        {
            return _activeGames.ContainsKey(channelId);
        }
    }
}
