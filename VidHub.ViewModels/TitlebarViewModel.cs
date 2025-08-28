using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VidHub.Platform.Generic;
using VidHub.Services.Logics.Interfaces;
using VidHub.Services.Modals.Interfaces;
using VidHub.Services.Settings.Interfaces;

namespace VidHub.ViewModels
{
    public partial class TitlebarViewModel(IVideoLoadService service, ISettingsService settings, IModalService modal) : ObservableRecipient
    {
        private bool CanOpenSidePanel() => !settings.OpenPanel;
        private bool CanCloseSidePanel() => settings.OpenPanel;


        public bool AllowToastNotifications
        {
            get => settings.AllowToastNotifications;
            set
            {
                if (settings.AllowToastNotifications == value) return;
                settings.AllowToastNotifications = value;
            }
        }

        public bool ConcurrentVideoLoading
        {
            get => settings.ConcurrentVideoLoading;
            set
            {
                if (settings.ConcurrentVideoLoading == value) return;
                settings.ConcurrentVideoLoading = value;
            }
        }

        public string Version => "VidHub 1.1.0";
        public string Copyright => $"© {DateTime.Now.Year} Mark Lehoczky";


        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task LoadFilesAsync()
        {
            await service.LoadFilesAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task LoadSingleFolderAsync()
        {
            await service.LoadFoldersAsync(false);
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task LoadAllFolderAsync()
        {
            await service.LoadFoldersAsync(true);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task ImportFileAsync()
        {
            await service.ImportFileAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task ExportFileAsync()
        {
            await service.ExportFileAsync();
        }


        [RelayCommand(CanExecute = nameof(CanOpenSidePanel))]
        private void OpenSidePanel()
        {
            settings.OpenPanel = true;
            ChangeSidePanelState();
        }

        [RelayCommand(CanExecute = nameof(CanCloseSidePanel))]
        private void CloseSidePanel()
        {
            settings.OpenPanel = false;
            ChangeSidePanelState();
        }


        [RelayCommand]
        private void OpenFeatures()
        {
            modal.IsFeatureModalVisible = true;
        }

        [RelayCommand]
        private void OpenLicenses()
        {
            modal.IsLicenseModalVisible = true;
        }


        public TitlebarViewModel() : this(
            Context.MainHost.Services.GetRequiredService<IVideoLoadService>(),
            Context.MainHost.Services.GetRequiredService<ISettingsService>(),
            Context.MainHost.Services.GetRequiredService<IModalService>())
        { }


        private void ChangeSidePanelState()
        {
            OpenSidePanelCommand.NotifyCanExecuteChanged();
            CloseSidePanelCommand.NotifyCanExecuteChanged();
        }
    }
}
