#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using Common;
using System;

namespace MemoryClock
{
    public static class Global
    {
        private static Settings.Settings BackupSettings { get; set;}
        public static Settings.Settings Settings { get; private set; }

        public static void Initialize()
        {
            // First perform any sychronous initialization
            InitializeSettings();

            // Then perform any off thread initialization
            InitializeOffthread();
        }

        private static void InitializeSettings()
        {
            Settings.Settings settings;
            if (CacheManager.TryLoad("Settings", out settings))
            {
                Settings = settings;
            }
            else
            {
                ResetSettings();
            }
        }

        private static void InitializeOffthread()
        {
        }

        public static void ResetSettings()
        {
            Settings = new Settings.Settings();
            Settings.LoadDefaults();
        }

        public static void PushSettings(Settings.Settings newSettings)
        {
            if (newSettings == null)
            {
                throw new ArgumentNullException("newSettings");
            }

            BackupSettings = Settings;
            Settings = newSettings;
        }

        public static void PopSettings()
        {
            if (BackupSettings == null)
            {
                throw new Exception("Settings have not been pushed");
            }

            Settings = BackupSettings;
            BackupSettings = null;
        }

        public static void Suspending()
        {
            SaveSettings();
        }

        public static void SaveSettings()
        {
            CacheManager.Save("Settings", Settings, true);
        }

        public static void DeleteSettings()
        {
            CacheManager.TryDelete("Settings");
        }
    }
}
