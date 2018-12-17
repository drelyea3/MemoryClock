#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using Common;

namespace MemoryClock
{
    public static class Global
    {
        public static Settings Settings { get; private set; }

        public static void Initialize()
        {
            // First perform any sychronous initialization
            InitializeSettings();

            // Then perform any off thread initialization
            InitializeOffthread();
        }

        private static void InitializeSettings()
        {
            Settings settings;
            if (CacheManager.TryLoad("Settings", out settings))
            {
                Settings = settings;
            }
            else
            {
                ResetSettings();
            }
        }

        public static void ResetSettings()
        {
            Settings = new Settings();
            Settings.LoadDefaults();
        }

        private static void InitializeOffthread()
        {
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
