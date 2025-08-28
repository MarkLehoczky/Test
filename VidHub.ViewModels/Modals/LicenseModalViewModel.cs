using Microsoft.Extensions.DependencyInjection;
using VidHub.Core;
using VidHub.Platform.Generic;
using VidHub.Services.Modals.Interfaces;

namespace VidHub.ViewModels.Modals
{
    public partial class LicenseModalViewModel(IModalService modal) : ModalViewModel
    {
        public override bool IsModalVisible { get => modal.IsLicenseModalVisible; set => modal.IsLicenseModalVisible = value; }

        public License ApplicationLicense => License.MIT("VidHub", "Mark Lehoczky", DateTime.Now.Year);

        public List<License> ExternalLicenses { get; } =
        [
            License.MIT("CommunityToolkit.Mvvm", "Microsoft", 2024),
            License.MIT("Microsoft.Extensions.Hosting", "Microsoft", 2025),
            License.MIT("Microsoft.Toolkit.Uwp.Notifications", "Microsoft", 2022),
            License.MicrosoftWindowsSDKBuildTools(2025),
            License.MicrosoftWindowsAppSDK(2025),
            License.MIT("System.Drawing.Common", "Microsoft", 2025),
            new License
            {
                Name = "Vectors and icons",
                Type = "CC Attribution License",
                Owner = "Solar Icons",
                Content =
                [
                    "You are free:",
                    "    • to share – to copy, distribute and transmit the work",
                    "    • to remix – to adapt the work",
                    " ",
                    "Under the following terms:",
                    "    • attribution – you must give appropriate credit, provide a link to the license, and indicate if changes were made",
                    "    • share alike – If you remix, transform, or build upon the material, you can distribute your work under any license"
                ]
            }
        ];

        public LicenseModalViewModel() : this(Context.MainHost.Services.GetRequiredService<IModalService>())
        { }
    }
}
