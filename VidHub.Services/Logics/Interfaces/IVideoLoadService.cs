using System.Collections.Concurrent;
using VidHub.Core;
using VidHub.Services.Base.Interfaces;
using Windows.Storage;

namespace VidHub.Services.Logics.Interfaces
{
    public interface IVideoLoadService : IUpdateService
    {
        string TransferDescription { get; }
        bool HasTransfer { get; }
        bool HasActiveTransfer { get; }
        int LoadedCount { get; }
        int TotalCount { get; }
        ConcurrentQueue<Transfer> Transfers { get; }
        Task LoadFilesAsync();
        Task LoadFoldersAsync(bool includeSubfolders);
        Task ImportFileAsync();
        Task ExportFileAsync();
        Task LoadExternal(IEnumerable<IStorageItem> items);
    }
}
