using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using VidHub.Core;
using VidHub.Platform.Generic;
using VidHub.Services.Logics.Interfaces;

namespace VidHub.ViewModels
{
    public partial class VideoCollectionViewModel(IVideoCollectionService service) : ObservableRecipient
    {
        public ObservableCollection<Video> Videos => service.DisplayedVideos;
        public bool IsEmpty => Videos.Count == 0;


        public VideoCollectionViewModel() : this(Context.MainHost.Services.GetRequiredService<IVideoCollectionService>())
        {
            Context.MainHost.Services.GetRequiredService<IVideoCollectionService>().SubscribeToUpdateEvent(UpdateProperties);
        }

        ~VideoCollectionViewModel()
        {
            service.UnsubscribeFromUpdateEvent(UpdateProperties);
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(IsEmpty));
        }
    }
}
