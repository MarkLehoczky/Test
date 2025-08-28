using VidHub.Services.Base.Interfaces;
using VidHub.Services.Modals.Interfaces;

namespace VidHub.Services.Modals
{
    public class ModalService(IMainService service) : IModalService
    {
        private bool isFeatureModalVisible = false;
        private bool isLicenseModalVisible = false;

        public bool IsFeatureModalVisible
        {
            get => isFeatureModalVisible;
            set
            {
                if (isFeatureModalVisible == value) return;
                isFeatureModalVisible = value;
                service.Update();
            }
        }

        public bool IsLicenseModalVisible
        {
            get => isLicenseModalVisible;
            set
            {
                if (isLicenseModalVisible == value) return;
                isLicenseModalVisible = value;
                service.Update();
            }
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
    }
}
