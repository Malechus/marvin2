using marvin2.Models;
using marvin2.Models.MediaFolderModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace marvin2.Services
{
    /// <summary>
    /// Scans a configured set of media folders on disk (movies, music, shows), compares
    /// the current subdirectory listing against the last known state persisted in the
    /// database, and reports any titles that have been added or removed. The database
    /// is updated to reflect the new state on every scan, so detection survives
    /// application restarts.
    /// </summary>
    public class MediaFolderService
    {
        private readonly ChoreContext _context;
        private readonly IConfigurationRoot _config;

        /// <summary>
        /// Creates a new instance of <see cref="MediaFolderService"/> and configures the EF
        /// <see cref="ChoreContext"/> using the provided <see cref="IConfigurationRoot"/>.
        /// The connection string is read from <c>Database:ConnectionString</c> in configuration.
        /// </summary>
        /// <param name="configurationRoot">Configuration root used to locate database connection settings and the <c>MediaFolders</c> section.</param>
        public MediaFolderService(IConfigurationRoot configurationRoot)
        {
            _config = configurationRoot;
            DbContextOptionsBuilder<ChoreContext> builder = new DbContextOptionsBuilder<ChoreContext>();
            builder.UseMySql(_config["Database:ConnectionString"], ServerVersion.AutoDetect(_config["Database:ConnectionString"]));
            _context = new ChoreContext(builder.Options);
        }

        /// <summary>
        /// Scans every folder configured under <c>MediaFolders</c> and returns a result
        /// describing any additions or removals found. Folders whose contents did not
        /// change are omitted from the result.
        /// </summary>
        /// <returns>A <see cref="MediaScanResult"/> containing one <see cref="FolderDiff"/> per changed folder.</returns>
        public MediaScanResult CheckForChanges()
        {
            return checkForChanges();
        }

        /// <summary>
        /// Internal implementation that iterates the configured folders, diffs each one
        /// against the database, and aggregates the changed folders into a result.
        /// </summary>
        /// <returns>A <see cref="MediaScanResult"/> containing one <see cref="FolderDiff"/> per changed folder.</returns>
        private MediaScanResult checkForChanges()
        {
            MediaScanResult result = new MediaScanResult();

            foreach (MediaFolderConfigEntry folder in getConfiguredFolders())
            {
                FolderDiff? diff = checkFolder(folder);

                if (diff != null)
                {
                    result.ChangedFolders.Add(diff);
                }
            }

            return result;
        }

        /// <summary>
        /// Reads the <c>MediaFolders</c> configuration section into a list of folder definitions.
        /// </summary>
        /// <returns>List of configured <see cref="MediaFolderConfigEntry"/> entries.</returns>
        private List<MediaFolderConfigEntry> getConfiguredFolders()
        {
            List<MediaFolderConfigEntry> folders = new List<MediaFolderConfigEntry>();

            foreach (IConfigurationSection section in _config.GetSection("MediaFolders").GetChildren())
            {
                folders.Add(new MediaFolderConfigEntry
                {
                    Key = section["Key"],
                    DisplayName = section["DisplayName"],
                    Path = section["Path"]
                });
            }

            return folders;
        }

        /// <summary>
        /// Scans a single folder's top-level subdirectories, diffs the listing against the
        /// database rows recorded for the folder's key, persists the additions/removals,
        /// and returns a diff describing the change (or <c>null</c> if nothing changed).
        /// </summary>
        /// <param name="folder">The folder definition to scan.</param>
        /// <returns>A <see cref="FolderDiff"/> describing the change, or <c>null</c> if the folder is unchanged or missing.</returns>
        private FolderDiff? checkFolder(MediaFolderConfigEntry folder)
        {
            if (!Directory.Exists(folder.Path))
            {
                return null;
            }

            List<string> currentTitles = Directory.GetDirectories(folder.Path)
                .Select(path => Path.GetFileName(path.TrimEnd(System.IO.Path.DirectorySeparatorChar)))
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList()!;

            List<MediaFolderItem> knownItems = _context.MediaFolderItems
                .Where(mfi => mfi.FolderKey == folder.Key)
                .ToList();

            List<string> knownTitles = knownItems.Select(item => item.Title).ToList();

            List<string> added = currentTitles.Except(knownTitles).ToList();
            List<string> removed = knownTitles.Except(currentTitles).ToList();

            if (added.Count == 0 && removed.Count == 0)
            {
                return null;
            }

            DateTime now = DateTime.UtcNow;

            foreach (string title in added)
            {
                _context.MediaFolderItems.Add(new MediaFolderItem
                {
                    FolderKey = folder.Key,
                    Title = title,
                    FirstSeenAt = now
                });
            }

            foreach (MediaFolderItem item in knownItems.Where(item => removed.Contains(item.Title)))
            {
                _context.MediaFolderItems.Remove(item);
            }

            _context.SaveChanges();

            return new FolderDiff
            {
                DisplayName = folder.DisplayName,
                Added = added,
                Removed = removed
            };
        }
    }
}
