﻿namespace VidHub.Services.Settings.Interfaces
{
    public interface ISettingsService
    {
        bool OpenPanel { get; set; }
        bool AllowToastNotifications { get; set; }
        bool ConcurrentVideoLoading { get; set; }
    }
}
