using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using VidHub.Core;
using VidHub.Platform.Generic;
using VidHub.Services.Logics.Interfaces;
using VidHub.Services.Settings.Interfaces;

namespace VidHub.ViewModels
{
    public partial class SidepanelViewModel(IVideoOrganizeService organizeService, IVideoLoadService loadService, ISettingsService settings) : ObservableRecipient
    {
        public bool OpenPanel
        {
            get => settings.OpenPanel;
        }

        #region Organizer section
        public string VideoCount => organizeService.GetFormattedVideoCount();

        public IEnumerable<string> SortOptions => organizeService.GetSortOptions();

        public string? CurrentSortOption
        {
            get => organizeService.CurrentSortOption;
            set => organizeService.CurrentSortOption = value;
        }


        public string? SearchText
        {
            get => organizeService.SearchText;
            set => organizeService.SearchText = value;
        }

        public bool FilterDate
        {
            get => organizeService.FilterDate;
            set => organizeService.FilterDate = value;
        }
        public DateTimeOffset? StartDate
        {
            get => organizeService.StartDate;
            set => organizeService.StartDate = value;
        }
        public DateTimeOffset? EndDate
        {
            get => organizeService.EndDate;
            set => organizeService.EndDate = value;
        }

        public bool FilterDuration
        {
            get => organizeService.FilterDuration;
            set => organizeService.FilterDuration = value;
        }
        public TimeSpan? MinDuration
        {
            get => organizeService.MinDuration;
            set => organizeService.MinDuration = value;
        }
        public TimeSpan? MaxDuration
        {
            get => organizeService.MaxDuration;
            set => organizeService.MaxDuration = value;
        }
        #endregion


        #region Transfer section
        public string TransferDescription => loadService.TransferDescription;
        public bool HasTransfer => loadService.HasTransfer;
        public bool HasActiveTransfer => loadService.HasActiveTransfer;
        public int LoadedCount => loadService.LoadedCount;
        public int TotalCount => loadService.TotalCount;
        public bool Indeterminate => TotalCount - LoadedCount == 0;
        #endregion


        public SidepanelViewModel() : this(
            Context.MainHost.Services.GetRequiredService<IVideoOrganizeService>(),
            Context.MainHost.Services.GetRequiredService<IVideoLoadService>(),
            Context.MainHost.Services.GetRequiredService<ISettingsService>())
        {
            Context.MainHost.Services.GetRequiredService<IVideoOrganizeService>().SubscribeToUpdateEvent(UpdateProperties);
        }

        ~SidepanelViewModel()
        {
            Context.MainHost.Services.GetRequiredService<IVideoOrganizeService>().UnsubscribeFromUpdateEvent(UpdateProperties);
        }


        public void UpdateProperties()
        {
            OnPropertyChanged(nameof(OpenPanel));
            OnPropertyChanged(nameof(VideoCount));
            OnPropertyChanged(nameof(TransferDescription));
            OnPropertyChanged(nameof(HasTransfer));
            OnPropertyChanged(nameof(HasActiveTransfer));
            OnPropertyChanged(nameof(LoadedCount));
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(Indeterminate));
        }
    }
}
