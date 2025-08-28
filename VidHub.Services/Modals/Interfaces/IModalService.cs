using VidHub.Services.Base.Interfaces;

namespace VidHub.Services.Modals.Interfaces
{
    public interface IModalService : IUpdateService
    {
        bool IsFeatureModalVisible { get; set; }
        bool IsLicenseModalVisible { get; set; }
    }
}
