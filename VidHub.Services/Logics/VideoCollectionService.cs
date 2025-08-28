using System.Collections.ObjectModel;
using VidHub.Core;
using VidHub.Services.Base.Interfaces;
using VidHub.Services.Logics.Interfaces;

namespace VidHub.Services.Logics
{
    public class VideoCollectionService : IVideoCollectionService
    {
        private readonly object locker = new();
        private readonly IMainService service;
        public ObservableCollection<Video> DisplayedVideos { get; } = [];


        public VideoCollectionService(IMainService service)
        {
            this.service = service;
            SubscribeToUpdateEvent(UpdateDisplayedVideos);
        }

        ~VideoCollectionService()
        {
            UnsubscribeFromUpdateEvent(UpdateDisplayedVideos);
        }


        public void SubscribeToUpdateEvent(Action action)
        {
            service.SubscribeToUpdateEvent(action);
        }

        public void UnsubscribeFromUpdateEvent(Action action)
        {
            service.UnsubscribeFromUpdateEvent(action);
        }

        public void Update()
        {
            service.Update();
        }


        private void UpdateDisplayedVideos()
        {
            lock (locker)
            {
                var nextDisplayVideos = service.GetAllVideos().Where(service.Predicate).Order(service.Comparer).ToList();

                for (int i = 0; i < Math.Min(DisplayedVideos.Count, nextDisplayVideos.Count); i++)
                {
                    if (!Equals(DisplayedVideos[i], nextDisplayVideos[i]))
                    {
                        DisplayedVideos[i] = nextDisplayVideos[i];
                    }
                }

                while (DisplayedVideos.Count > nextDisplayVideos.Count)
                {
                    DisplayedVideos.RemoveAt(DisplayedVideos.Count - 1);
                }

                for (int i = DisplayedVideos.Count; i < nextDisplayVideos.Count; i++)
                {
                    DisplayedVideos.Add(nextDisplayVideos[i]);
                }
            }
        }
    }
}
