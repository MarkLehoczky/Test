using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VidHub.Platform.Generic;
using VidHub.Services.Modals.Interfaces;

namespace VidHub.ViewModels.Modals
{
    public partial class ModalViewModel(IModalService modal) : ObservableRecipient
    {
        virtual public bool IsModalVisible { get; set; }


        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
            OnPropertyChanged(nameof(IsModalVisible));
        }


        public ModalViewModel() : this(Context.MainHost.Services.GetRequiredService<IModalService>())
        {
            Context.MainHost.Services.GetRequiredService<IModalService>().SubscribeToUpdateEvent(UpdateProperties);
        }

        ~ModalViewModel()
        {
            Context.MainHost.Services.GetRequiredService<IModalService>().UnsubscribeFromUpdateEvent(UpdateProperties);
        }


        public void UpdateProperties()
        {
            OnPropertyChanged(nameof(IsModalVisible));
        }
    }
}
