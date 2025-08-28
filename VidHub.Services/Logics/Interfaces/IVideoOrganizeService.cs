using VidHub.Services.Base.Interfaces;

namespace VidHub.Services.Logics.Interfaces
{
    public interface IVideoOrganizeService : IUpdateService
    {
        string? CurrentSortOption { get; set; }
        string? SearchText { get; set; }
        bool FilterDate { get; set; }
        DateTimeOffset? StartDate { get; set; }
        DateTimeOffset? EndDate { get; set; }
        bool FilterDuration { get; set; }
        TimeSpan? MinDuration { get; set; }
        TimeSpan? MaxDuration { get; set; }
        string GetFormattedVideoCount();
        IEnumerable<string> GetSortOptions();
    }
}
