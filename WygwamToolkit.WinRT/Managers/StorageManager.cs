using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using WygwamToolkit.Common.Managers;

namespace PMV.Windows8.Common.Managers
{
    public class StorageManager : AStorageManager
    {
        public static StorageFolder DefaultFolder = ApplicationData.Current.RoamingFolder;
        public static ApplicationDataContainer DefaultSettings = ApplicationData.Current.RoamingSettings;

        #region Settings
        protected override async Task<bool> ClearPlatformAsync(string key)
        {
            return await Task.Run(() =>
            {
                if (DefaultSettings.Values.ContainsKey(key))
                    return DefaultSettings.Values.Remove(key);

                return false;
            });
        }

        protected override async Task<T> LoadPlatformAsync<T>(string key)
        {
            return await Task.Run(() =>
            {
                if (!DefaultSettings.Values.ContainsKey(key))
                    return default(T);

                return (T)DefaultSettings.Values[key];
            });
        }

        protected override async Task<bool> SavePlatformAsync(string key, object data)
        {
            return await Task.Run(() =>
                {
                    if (null == data)
                        return false;

                    if (DefaultSettings.Values.ContainsKey(key))
                        DefaultSettings.Values[key] = data;
                    else
                        DefaultSettings.Values.Add(key, data);

                    return true;
                });
        }
        #endregion

        #region Data
        protected override async Task<bool> SaveDataPlatformAsync(string path, object data)
        {
            const CreationCollisionOption option = CreationCollisionOption.ReplaceExisting;

            try
            {
                var file = await DefaultFolder.CreateFileAsync(path, option);
                var saveData = new MemoryStream();

                var x = new XmlSerializer(data.GetType());
                x.Serialize(saveData, data);

                if (saveData.Length > 0)
                {
                    using (var fileStream = await file.OpenStreamForWriteAsync())
                    {
                        saveData.Seek(0, SeekOrigin.Begin);
                        await saveData.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        protected override async Task<T> LoadDataPlatformAsync<T>(string path)
        {
            try
            {
                var file = await DefaultFolder.GetFileAsync(path);

                using (var inStream = await file.OpenSequentialReadAsync())
                {
                    var x = new XmlSerializer(typeof(T));
                    return (T)x.Deserialize(inStream.AsStreamForRead());
                }
            }
            catch
            {
                return default(T);
            }
        }

        protected override async Task<bool> ClearDataPlatformAsync(string path)
        {
            try
            {
                var file = await DefaultFolder.GetFileAsync(path);
                await file.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
            }

            return false;
        }
        #endregion
    }
}
