using Microsoft.Extensions.DependencyInjection;
using VidHub.Core;
using VidHub.Platform.Generic;
using VidHub.Services.Modals.Interfaces;

namespace VidHub.ViewModels.Modals
{
    public partial class FeatureModalViewModel(IModalService modal) : ModalViewModel
    {
        public override bool IsModalVisible { get => modal.IsFeatureModalVisible; set => modal.IsFeatureModalVisible = value; }

        public List<VersionChanges> VersionChanges { get; set; } =
        [
            new VersionChanges
            {
                Version = "v1.1.0",
                Features =
                [
                    "⚡ Concurrent video loading for speed",
                    "🔔 Toast notifications for completed video loading",
                    "🖥 Taskbar progress state integration",
                    "😊 Added this Feature Details modal",
                    "📜 Added License modals",
                    "📋 Clipboard loading",
                    "🖱 Drag & Drop loading",
                    "📂 Multiple file collecting & loading support"
                ],
                InternalChanges =
                [
                    "✅ Build Verification CI",
                    "📂 Reorganized file structure for clarity"
                ]
            },

            new VersionChanges
            {
                Version = "v1.0.0",
                Features =
                [
                    "🚀 Core models implemented",
                    "🖥 Clean UI setup",
                    "📥 Basic video file loading",
                    "📂 Load video files",
                    "📦 Video collection import & export options",
                    "🔍 Basic sorting & filtering",
                    "🔄 Transfer indicator for loading, importing & exporting",
                    "🖼 Added icons & licenses for publishing"
                ],
                InternalChanges =
                [
                    "📁 Project structure initialized"
                ]
            }
        ];

        public FeatureModalViewModel() : this(Context.MainHost.Services.GetRequiredService<IModalService>())
        { }
    }
}
