namespace marvin2.Models.MediaFolderModels
{
    /// <summary>
    /// Represents one entry of the <c>MediaFolders</c> configuration section: a monitored
    /// folder's identifying key, display name, and filesystem path.
    /// </summary>
    public class MediaFolderConfigEntry
    {
        /// <summary>
        /// Short identifier for the folder (e.g. "Movies"). Stored alongside each
        /// <see cref="MediaFolderItem"/> to associate it with this folder.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Human-friendly name used when formatting change notifications (e.g. "Movies").
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Absolute filesystem path to scan for subdirectories.
        /// </summary>
        public string Path { get; set; }
    }

    /// <summary>
    /// Describes the additions and removals detected for a single monitored media folder
    /// during a scan. Only produced for folders where the contents actually changed.
    /// </summary>
    public class FolderDiff
    {
        /// <summary>
        /// Human-friendly name of the folder (e.g. "Movies"), used when formatting
        /// the Discord notification message.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Titles (subdirectory names) that are present on disk now but were not
        /// previously recorded.
        /// </summary>
        public List<string> Added { get; set; } = new List<string>();

        /// <summary>
        /// Titles (subdirectory names) that were previously recorded but are no
        /// longer present on disk.
        /// </summary>
        public List<string> Removed { get; set; } = new List<string>();
    }

    /// <summary>
    /// Aggregate result of a full media folder scan across all configured folders.
    /// Contains one <see cref="FolderDiff"/> per folder that changed; unchanged folders
    /// are omitted.
    /// </summary>
    public class MediaScanResult
    {
        /// <summary>
        /// The list of folder-level diffs for folders whose contents changed since the
        /// last scan. Empty if nothing changed.
        /// </summary>
        public List<FolderDiff> ChangedFolders { get; set; } = new List<FolderDiff>();

        /// <summary>
        /// True if at least one monitored folder had additions or removals.
        /// </summary>
        public bool HasChanges => ChangedFolders.Count > 0;
    }
}
