using System.Collections.Concurrent;
using System.Text.Json;
using VidHub.Core;
using VidHub.Platform.Generic;
using VidHub.Services.Base.Interfaces;
using VidHub.Services.Logics.Interfaces;
using VidHub.Services.Settings.Interfaces;
using VidHub.Services.System.Interfaces;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace VidHub.Services.Logics
{
    public class VideoLoadService(IMainService service, ISystemManager manager, ISettingsService settings) : IVideoLoadService
    {
        private readonly object locker = new();
        private readonly JsonSerializerOptions jsonOptions = new()
        {
            AllowTrailingCommas = false,
            WriteIndented = true,
            IndentSize = 4
        };

        public bool HasTransfer => !Transfers.IsEmpty;
        public bool HasActiveTransfer => Transfers.Any(t => t.IsActive);

        public string TransferDescription =>
            Transfers.Where(t => t.IsActive).All(t => t.TotalCount > 0)
                ? "Loading videos"
                : Transfers.Where(t => t.IsActive).All(t => t.TotalCount == 0)
                    ? "Collecting videos"
                    : Transfers.Any(t => t.IsLoading)
                        ? "Collecting and loading videos"
                        : "Video loading finished";

        public ConcurrentQueue<Transfer> Transfers { get; private set; } = [];

        public int LoadedCount => Transfers.Sum(t => t.LoadedCount);

        public int TotalCount => Transfers.Sum(t => t.TotalCount);


        public async Task LoadFilesAsync()
        {
            IReadOnlyList<StorageFile> files = await PickFilesOpen("Load", Video.ExtensionTypes);

            if (files.Count > 0)
            {
                await Task.Run(() =>
                {
                    var index = Transfers.Count;
                    var transfer = new Transfer(files.Count);
                    Transfers.Enqueue(transfer);
                    manager.SetTaskbar(Transfers);
                    Update();

                    AddFilesToVideoCollection(index, files);

                    manager.SetTaskbar(Transfers);
                    Update();
                    TransferCleanup();
                });
            }
        }

        public async Task LoadFoldersAsync(bool includeSubfolders)
        {
            StorageFolder? folder = await PickFolderOpen("Load");

            if (folder != null)
            {
                await Task.Run(async () =>
                {
                    var index = Transfers.Count;
                    var transfer = new Transfer();
                    Transfers.Enqueue(transfer);
                    manager.SetTaskbar(Transfers);
                    Update();

                    var files = await CollectFilesAsync(folder, includeSubfolders, Video.ExtensionTypes);
                    Transfers.ElementAt(index).AddTotalCount(files.Count());
                    AddFilesToVideoCollection(index, files);

                    manager.SetTaskbar(Transfers);
                    Update();
                    TransferCleanup();
                });
            }
        }

        public async Task ImportFileAsync()
        {
            StorageFile? file = await PickFileOpen("Import", [".json"]);

            if (file != null)
            {
                await Task.Run(async () =>
                {
                    var fileContent = await FileIO.ReadTextAsync(file);
                    List<Video>? importedCollection = JsonSerializer.Deserialize<List<Video>>(fileContent);
                    foreach (var video in importedCollection ?? Enumerable.Empty<Video>())
                    {
                        service.AddVideo(video);
                    }
                });
            }
        }

        public async Task ExportFileAsync()
        {
            StorageFile? file = await PickFileSave("Export", ".json", "VidHub Collection");

            if (file != null)
            {
                await Task.Run(async () =>
                {
                    var fileContent = JsonSerializer.Serialize(service.GetAllVideos(), jsonOptions);
                    await FileIO.WriteTextAsync(file, fileContent);
                });
            }
        }


        public async Task LoadExternal(IEnumerable<IStorageItem> items)
        {
            if (items.Any())
            {
                await Task.Run(async () =>
                {
                    var index = Transfers.Count;
                    var transfer = new Transfer();
                    Transfers.Enqueue(transfer);
                    manager.SetTaskbar(Transfers);
                    Update();

                    var files = items.OfType<StorageFile>().Where(f => Video.ExtensionTypes.Contains(f.FileType)).ToList();
                    foreach (var folder in items.OfType<StorageFolder>())
                    {
                        files.AddRange(await CollectFilesAsync(folder, true, Video.ExtensionTypes));
                    }
                    if (files.Count > 0)
                    {
                        Transfers.ElementAt(index).AddTotalCount(files.Count);
                        AddFilesToVideoCollection(index, files);
                    }

                    manager.SetTaskbar(Transfers);
                    Update();
                    TransferCleanup();
                });
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


        private static async Task<IReadOnlyList<StorageFile>> PickFilesOpen(string commitButtonText, List<string> fileTypeFilters)
        {
            var picker = new FileOpenPicker
            {
                CommitButtonText = commitButtonText,
                SuggestedStartLocation = PickerLocationId.HomeGroup,
                ViewMode = PickerViewMode.Thumbnail
            };
            foreach (var filter in fileTypeFilters)
            {
                picker.FileTypeFilter.Add(filter);
            }

            InitializeWithWindow.Initialize(picker, Context.MainWindow.HWND);
            return await picker.PickMultipleFilesAsync();
        }

        private static async Task<StorageFolder?> PickFolderOpen(string commitButtonText)
        {
            var picker = new FolderPicker
            {
                CommitButtonText = commitButtonText,
                SuggestedStartLocation = PickerLocationId.VideosLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };

            InitializeWithWindow.Initialize(picker, Context.MainWindow.HWND);
            return await picker.PickSingleFolderAsync();
        }

        private static async Task<StorageFile?> PickFileOpen(string commitButtonText, List<string> fileTypeFilters)
        {
            var picker = new FileOpenPicker
            {
                CommitButtonText = commitButtonText,
                SuggestedStartLocation = PickerLocationId.HomeGroup,
                ViewMode = PickerViewMode.Thumbnail
            };
            foreach (var filter in fileTypeFilters)
            {
                picker.FileTypeFilter.Add(filter);
            }

            InitializeWithWindow.Initialize(picker, Context.MainWindow.HWND);
            return await picker.PickSingleFileAsync();
        }

        private static async Task<StorageFile?> PickFileSave(string commitButtonText, string defaultFileExtension, string suggestedFileName)
        {
            var picker = new FileSavePicker
            {
                CommitButtonText = commitButtonText,
                DefaultFileExtension = defaultFileExtension,
                SuggestedFileName = suggestedFileName,
                SuggestedStartLocation = PickerLocationId.HomeGroup
            };
            picker.FileTypeChoices.Add(suggestedFileName, [defaultFileExtension]);

            InitializeWithWindow.Initialize(picker, Context.MainWindow.HWND);
            return await picker.PickSaveFileAsync();
        }


        private static async Task<IEnumerable<StorageFile>> CollectFilesAsync(StorageFolder folder, bool includeSubfolders, List<string> fileTypeFilters)
        {
            var files = new List<StorageFile>();
            files.AddRange(await folder.GetFilesAsync());

            if (includeSubfolders)
            {
                foreach (var subfolder in await folder.GetFoldersAsync())
                {
                    files.AddRange(await CollectFilesAsync(subfolder, includeSubfolders, fileTypeFilters));
                }
            }

            return files.Where(f => fileTypeFilters.Contains(f.FileType, StringComparer.OrdinalIgnoreCase));
        }

        private void AddFilesToVideoCollection(int index, IEnumerable<StorageFile> files)
        {
            lock (locker)
            {
                Transfers.ElementAt(index).IsLoading = true;

                if (settings.ConcurrentVideoLoading)
                {
                    Parallel.ForEach(files, file =>
                    {
                        var video = new Video(file.Path);
                        video.TryLoad();
                        service.AddVideo(video);
                        Transfers.ElementAt(index).Increment();
                        manager.SetTaskbar(Transfers);
                    });
                }
                else
                {
                    foreach (var file in files)
                    {
                        var video = new Video(file.Path);
                        video.TryLoad();
                        service.AddVideo(video);
                        Transfers.ElementAt(index).Increment();
                        manager.SetTaskbar(Transfers);
                    }
                }

                Transfers.ElementAt(index).IsLoading = false;
                Transfers.ElementAt(index).IsActive = false;
                manager.SetTaskbar(Transfers);
                Update();
            }
        }

        private void TransferCleanup()
        {
            if (Transfers.All(t => !t.IsActive))
            {
                manager.DisplayToast("Video loading finished!", $"{Transfers.Sum(t => t.LoadedCount)} videos were loaded successfully.");
                while (Transfers.TryDequeue(out _)) ;
            }
        }
    }
}
