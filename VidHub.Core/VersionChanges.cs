namespace VidHub.Core
{
    public class VersionChanges
    {
        public string Version { get; set; } = string.Empty;

        public bool HasFeatures => Features.Count > 0;
        public bool HasBugfixes => Bugfixes.Count > 0;
        public bool HasInternalChanges => InternalChanges.Count > 0;

        public List<string> Features { get; set; } = [];
        public List<string> Bugfixes { get; set; } = [];
        public List<string> InternalChanges { get; set; } = [];
    }
}
