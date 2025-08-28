﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;
using VidHub.Platform.Generic;
using VidHub.Platform.Generic.Interfaces;
using VidHub.Services.Base;
using VidHub.Services.Base.Interfaces;
using VidHub.Services.Logics;
using VidHub.Services.Logics.Interfaces;
using VidHub.Services.Modals;
using VidHub.Services.Modals.Interfaces;
using VidHub.Services.Settings;
using VidHub.Services.Settings.Interfaces;
using VidHub.Services.System;
using VidHub.Services.System.Interfaces;
using VidHub.ViewModels;
using WinRT.Interop;

namespace VidHub.WinUI
{
    public class WindowContext(Window window) : IWindowContext
    {
        public object Window => window;
        public nint HWND => WindowNative.GetWindowHandle(window);
        public bool IsActive { get; set; } = false;

        public bool TryEnqueue(Action callback)
        {
            return window.DispatcherQueue.TryEnqueue(callback.Invoke);
        }
    }

    public partial class App : Application
    {
        private Window? _window;

        public App()
        {
            InitializeComponent();
            Context.MainHost = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IMainService, MainService>();
                    services.AddSingleton<ISystemManager, SystemManager>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IModalService, ModalService>();
                    services.AddSingleton<IVideoLoadService, VideoLoadService>();
                    services.AddSingleton<IVideoOrganizeService, VideoOrganizeService>();
                    services.AddSingleton<IVideoCollectionService, VideoCollectionService>();
                    services.AddTransient<TitlebarViewModel>();
                    services.AddTransient<VideoCollectionViewModel>();
                })
                .Build();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
            Context.MainWindow = new WindowContext(_window);
            _window.Activated += (s, e) => Context.MainWindow.IsActive = e.WindowActivationState != WindowActivationState.Deactivated;
        }
    }
}
