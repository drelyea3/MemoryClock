#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using Common;
using System;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Xaml.Media;

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

            foreach (var pair in ((App)App.Current).GetWorkers())
            {
                var key = pair.Item1;
                var worker = pair.Item2;
                worker.IsEnabled = worker.IsAlwaysEnabled || Settings.EnabledWorkers.Contains(key);
            }

            // Machine-specific initialization
            // get the device manufacturer and model name
            var eas = new EasClientDeviceInformation();
            var DeviceManufacturer = eas.SystemManufacturer;
            var DeviceModel = eas.SystemProductName;
            var SKU = eas.SystemSku;
            var result = string.CompareOrdinal(SKU,"RPi3");
            if (SKU.StartsWith("RPi") && string.CompareOrdinal(SKU,"RPi3") < 0)
            {
                // Raspberry Pi2 devices can't handle AcrylicBrush effects, so force fallback
                ((AcrylicBrush)App.Current.Resources["DarkSmoke"]).AlwaysUseFallback = true;
                ((AcrylicBrush)App.Current.Resources["LightSmoke"]).AlwaysUseFallback = true;
            }

            Logger.Log($"SKU {SKU} Manu {DeviceManufacturer} Model {DeviceModel}");
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
