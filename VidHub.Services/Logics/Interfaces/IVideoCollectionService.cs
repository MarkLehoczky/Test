using System.Collections.ObjectModel;
using VidHub.Core;
using VidHub.Services.Base.Interfaces;

namespace VidHub.Services.Logics.Interfaces
{
    public interface IVideoCollectionService : IUpdateService
    {
        ObservableCollection<Video> DisplayedVideos { get; }
    }
}
