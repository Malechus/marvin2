using System.Text;
using System.Timers;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using marvin2.Models.MediaFolderModels;
using marvin2.Services;

namespace marvin2.discord.Services
{
    /// <summary>
    /// Periodically scans the configured media folders (via <see cref="MediaFolderService"/>)
    /// and posts a Discord message describing any additions or removals. Runs on a fixed
    /// interval timer, mirroring the structure of <see cref="RaccoonGameScheduler"/>, but
    /// without randomized scheduling since the check interval here is constant.
    /// </summary>
    public class MediaFolderMonitorScheduler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly MediaFolderService _mediaFolderService;
        private readonly IConfigurationRoot _config;
        private readonly ILogger<MediaFolderMonitorScheduler> _logger;
        private System.Timers.Timer _timer;
        private readonly double _checkIntervalMs = 1800000; // 30 minutes

        /// <summary>
        /// Initializes a new instance of <see cref="MediaFolderMonitorScheduler"/>.
        /// </summary>
        /// <param name="discordClient">Discord socket client used to resolve and post to the alerts channel.</param>
        /// <param name="mediaFolderService">Service used to scan media folders and detect changes.</param>
        /// <param name="configurationRoot">Configuration root used to look up the alerts channel id.</param>
        /// <param name="logger">Logger for recording scan activity and errors.</param>
        public MediaFolderMonitorScheduler(
            DiscordSocketClient discordClient,
            MediaFolderService mediaFolderService,
            IConfigurationRoot configurationRoot,
            ILogger<MediaFolderMonitorScheduler> logger)
        {
            _discordClient = discordClient;
            _mediaFolderService = mediaFolderService;
            _config = configurationRoot;
            _logger = logger;
        }

        /// <summary>
        /// Starts the media folder monitor on a fixed 30-minute interval.
        /// This should be called once during bot startup.
        /// </summary>
        public void StartAsync()
        {
            _logger.LogInformation("MediaFolderMonitorScheduler: Starting scheduler");

            _timer = new System.Timers.Timer(_checkIntervalMs);
            _timer.Elapsed += onTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();

            _logger.LogInformation($"MediaFolderMonitorScheduler: Next scan scheduled in {_checkIntervalMs / 1000 / 60} minutes");
            _logger.LogInformation("MediaFolderMonitorScheduler: Running startup scan");
            _ = scanAndNotifyAsync();
        }

        /// <summary>
        /// Stops the media folder monitor gracefully.
        /// </summary>
        public void StopAsync()
        {
            _logger.LogInformation("MediaFolderMonitorScheduler: Stopping scheduler");
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Handles the timer elapsed event by triggering a scan.
        /// </summary>
        private void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _ = scanAndNotifyAsync();
        }

        /// <summary>
        /// Scans the configured media folders and, if any changed, posts a message to the
        /// configured alerts channel describing the additions and removals per folder.
        /// </summary>
        private async Task scanAndNotifyAsync()
        {
            try
            {
                MediaScanResult result = _mediaFolderService.CheckForChanges();

                if (!result.HasChanges)
                {
                    _logger.LogInformation("MediaFolderMonitorScheduler: No changes detected");
                    return;
                }

                string message = formatChangeMessage(result);

                ISocketMessageChannel channel = await _discordClient.GetChannelAsync(
                    ulong.Parse(_config["Discord:Channels:Media_Alerts"])) as ISocketMessageChannel;

                if (channel == null)
                {
                    _logger.LogWarning("MediaFolderMonitorScheduler: Media_Alerts channel could not be resolved");
                    return;
                }

                await channel.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MediaFolderMonitorScheduler: Error scanning media folders - {ex.Message}");
            }
        }

        /// <summary>
        /// Builds a Discord message describing the changes in <paramref name="result"/>,
        /// delineated by folder, with additions and removals listed separately.
        /// </summary>
        /// <param name="result">The scan result containing the changed folders.</param>
        /// <returns>A formatted message ready to post to the alerts channel.</returns>
        private string formatChangeMessage(MediaScanResult result)
        {
            StringBuilder builder = new StringBuilder();

            foreach (FolderDiff diff in result.ChangedFolders)
            {
                if (diff.Added.Count > 0)
                {
                    builder.AppendLine($"### New {diff.DisplayName}:{Environment.NewLine + "- "}{string.Join(Environment.NewLine + "- ", diff.Added)}");
                }

                if (diff.Removed.Count > 0)
                {
                    builder.AppendLine($"### Removed {diff.DisplayName}:{Environment.NewLine + "- "}{string.Join(Environment.NewLine + "- ", diff.Removed)}");
                }

                builder.AppendLine();
            }

            return builder.ToString().TrimEnd();
        }
    }
}
