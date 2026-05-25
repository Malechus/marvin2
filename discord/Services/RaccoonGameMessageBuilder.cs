using Discord;

namespace marvin2.discord.Services
{
    /// <summary>
    /// Provides static methods for constructing Discord embed messages related to the raccoon game.
    /// Handles formatting of the initial game prompt and winner announcement messages.
    /// </summary>
    public static class RaccoonGameMessageBuilder
    {
        /// <summary>
        /// Generates the initial raccoon game message with ASCII art and action prompt.
        /// Prompts players to use /hunt or /shoo commands to catch or scare away the raccoon.
        /// </summary>
        /// <returns>An <see cref="Embed"/> containing the raccoon game prompt with ASCII art.</returns>
        public static Embed BuildGameMessage()
        {
            return buildGameMessage();
        }

        /// <summary>
        /// Internal implementation that constructs the initial game embed.
        /// </summary>
        /// <returns>An <see cref="Embed"/> for the raccoon game.</returns>
        private static Embed buildGameMessage()
        {
            string asciiRaccoon = """
                ```
                    ▄▄▄▄▄▄
                   /      \
                  | (●)●) |
                  |    >   |
                  |  \_/  |
                   \      /
                    ▀▀▀▀▀▀
                ```
                """;

            var embed = new EmbedBuilder()
                .WithColor(Color.Orange)
                .WithTitle("🦝 A wild raccoon has appeared!")
                .WithDescription(asciiRaccoon)
                .AddField("What do you do?", "Use **/hunt** to catch it or **/shoo** to scare it away!\nFirst person to respond wins a point!")
                .WithFooter("Choose quickly!")
                .Build();

            return embed;
        }

        /// <summary>
        /// Generates a congratulations message for the winner with their final score.
        /// </summary>
        /// <param name="winner">The user who won the game.</param>
        /// <param name="score">The winner's current score.</param>
        /// <returns>An <see cref="Embed"/> announcing the winner and their score.</returns>
        public static Embed BuildWinnerMessage(IUser winner, int score)
        {
            return buildWinnerMessage(winner, score);
        }

        /// <summary>
        /// Internal implementation that constructs the winner announcement embed.
        /// </summary>
        /// <param name="winner">The user who won the game.</param>
        /// <param name="score">The winner's current score.</param>
        /// <returns>An <see cref="Embed"/> announcing the winner.</returns>
        private static Embed buildWinnerMessage(IUser winner, int score)
        {
            var embed = new EmbedBuilder()
                .WithColor(Color.Gold)
                .WithTitle("🎉 Raccoon Caught!")
                .WithDescription($"**{winner.Username}** successfully caught the raccoon!")
                .AddField("Current Score", score.ToString(), inline: true)
                .WithThumbnailUrl(winner.GetAvatarUrl() ?? winner.GetDefaultAvatarUrl())
                .WithFooter("Raccoon was faster next time...")
                .Build();

            return embed;
        }
    }
}
