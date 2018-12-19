using Newtonsoft.Json;
using System;
using System.Diagnostics;
using Windows.Storage;

namespace Common
{
    public static class CacheManager
    {
        public const string Suffix = ".cache.json";

        public static bool TryLoad<T>(string fileName, out T result) where T : class
        {
            fileName = fileName + Suffix;

            var folder = ApplicationData.Current.LocalFolder;
            StorageFile file = folder.TryGetItemAsync(fileName).AsTask().Result as StorageFile;

            if (file != null)
            {
                string json = FileIO.ReadTextAsync(file).AsTask().Result;
                Debug.WriteLine(json);
                result = JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                result = null;
            }

            // It could be that the file is empty
            return result != null;
        }

        public static void Save(string fileName, object data, bool serialize)
        {
            fileName = fileName + Suffix;

            var folder = ApplicationData.Current.LocalFolder;
            var file = folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask().Result as StorageFile;

            string text = null;

            if (!serialize)
            {
                text = data as string;
            }
            else
            {
                text = JsonConvert.SerializeObject(data, Formatting.Indented);
            }

            FileIO.WriteTextAsync(file, text).AsTask().Wait();
        }

        public static bool TryDelete(string fileName)
        {
            fileName = fileName + Suffix;

            var folder = ApplicationData.Current.LocalFolder;
            StorageFile file = folder.TryGetItemAsync(fileName).AsTask().Result as StorageFile;
            if (file != null)
            {
                Debug.WriteLine("Deleting " + file.Name);
                file.DeleteAsync().AsTask().Wait();
                return true;
            }

            return false;
        }

        public static void DeleteAll(Func<string, bool> filter = null)
        {
            var folder = ApplicationData.Current.LocalFolder;

            var items = folder.GetItemsAsync().AsTask().Result;

            foreach (var item in items)
            {
                var file = item as StorageFile;
                if (file != null && file.Name.EndsWith(Suffix) && (filter == null || filter(file.Name)))
                {
                    Debug.WriteLine("Deleting " + file.Name);
                    file.DeleteAsync().AsTask().Wait();
                }
            }
        }
    }
}
