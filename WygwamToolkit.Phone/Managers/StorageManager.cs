using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WygwamToolkit.Common.Managers;

namespace PMV.Mobile.Common.Managers
{
    public class StorageManager : AStorageManager
    {
        #region Settings
        protected override async Task<bool> SavePlatformAsync(string key, object data)
        {
            return await Task.Run(() =>
                {
                    if (null == data)
                        return false;

                    var store = IsolatedStorageSettings.ApplicationSettings;
                    if (store.Contains(key))
                        store[key] = data;
                    else
                        store.Add(key, data);

                    store.Save();
                    return true;
                });
        }

        protected override async Task<T> LoadPlatformAsync<T>(string key)
        {
            return await Task.Run(() =>
                {
                    var store = IsolatedStorageSettings.ApplicationSettings;
                    if (!store.Contains(key))
                        return default(T);

                    return (T)store[key];
                });
        }

        protected override async Task<bool> ClearPlatformAsync(string key)
        {
            return await Task.Run(() =>
                {
                    if (null == key)
                        return false;

                    var store = IsolatedStorageSettings.ApplicationSettings;
                    if (store.Contains(key))
                        store.Remove(key);
                    store.Save();
                    return true;
                });
        }
        #endregion

        #region Data
        protected override async Task<bool> SaveDataPlatformAsync(string path, object data)
        {
            try
            {
                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isoStore.FileExists(path))
                        isoStore.DeleteFile(path);

                    var saveData = new MemoryStream();
                    var x = new XmlSerializer(data.GetType());
                    x.Serialize(saveData, data);

                    if (saveData.Length > 0)
                    {
                        using (var fileStream = isoStore.CreateFile(path))
                        {
                            saveData.Seek(0, SeekOrigin.Begin);
                            await saveData.CopyToAsync(fileStream);
                            await fileStream.FlushAsync();
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        protected override async Task<T> LoadDataPlatformAsync<T>(string path)
        {
            return await Task.Run(() =>
                {
                    try
                    {
                        using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            if (isoStore.FileExists(path))
                                using (var inStream = isoStore.OpenFile(path, FileMode.Open).AsInputStream())
                                {
                                    var x = new XmlSerializer(typeof(T));
                                    object ret = x.Deserialize(inStream.AsStreamForRead());
                                    return (T)ret;
                                }
                        }
                    }
                    catch
                    {
                    }

                    return default(T);
                });
        }

        protected override async Task<bool> ClearDataPlatformAsync(string path)
        {
            return await Task.Run(() =>
                {
                    try
                    {
                        using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            if (isoStore.FileExists(path))
                            {
                                isoStore.DeleteFile(path);
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    return false;
                });
        }
        #endregion
    }
}
