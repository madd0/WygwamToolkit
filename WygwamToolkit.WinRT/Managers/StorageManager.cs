//-----------------------------------------------------------------------
// <copyright file="StorageManager.cs" company="Wygwam">
//     Copyright (c) 2013 Wygwam.
//     Licensed under the Microsoft Public License (Ms-PL) (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//
//         http://opensource.org/licenses/Ms-PL.html
//
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

namespace Wygwam.Windows.WinRT
{
    using global::Windows.Storage;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Wygwam.Windows.Storage;

    /// <summary>
    /// Extends <see cref="Wygwam.Windows.Storage.StorageManager"/> to store settings and objects in local or roaming storage.
    /// </summary>
    public class StorageManager : Wygwam.Windows.Storage.StorageManager
    {
        private static readonly CreationCollisionOption _defaultCollisionPolicy = CreationCollisionOption.ReplaceExisting;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageManager"/> class.
        /// </summary>
        public StorageManager()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageManager"/> class.
        /// </summary>
        /// <param name="defaultSerializer">The default serializer.</param>
        public StorageManager(IDataSerializer defaultSerializer)
            : base(defaultSerializer)
        {
        }

        /// <summary>
        /// Called when <see cref="M:SaveSettingAsync" /> is executed to store data in the settings container.
        /// </summary>
        /// <param name="key">The string to use as a key for the element to save.</param>
        /// <param name="data">The object that will be saved.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the value was successfully stored.
        /// </returns>
        protected override Task<bool> OnSaveSettingAsync(string key, object data, Storage.StorageType storageType)
        {
            return Task.Run<bool>(() =>
                {
                    if (null == data)
                    {
                        return false;
                    }

                    var store = GetSettingsContainer(storageType);

                    if (store.Values.ContainsKey(key))
                    {
                        store.Values[key] = data;
                    }
                    else
                    {
                        store.Values.Add(key, data);
                    }

                    return true;
                });
        }

        /// <summary>
        /// Called when <see cref="M:LoadSettingAsync{T}"/> is executed to load data from the settings container.
        /// </summary>
        /// <typeparam name="T">The type of the stored value.</typeparam>
        /// <param name="key">The string that was used as a key for the desired value.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>The value associated to the specified key.</returns>
        protected override Task<T> OnLoadSettingAsync<T>(string key, Storage.StorageType storageType)
        {
            return Task.Run<T>(() =>
            {
                var store = GetSettingsContainer(storageType);

                if (!store.Values.ContainsKey(key))
                {
                    return default(T);
                }

                return (T)store.Values[key];
            });
        }

        /// <summary>
        /// Called when <see cref="M:RemoveSettingAsync" /> is executed to delete a value from the settings container.
        /// </summary>
        /// <param name="key">The string that identifies the value to delete.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the value was successfully deleted.
        /// </returns>
        protected override Task<bool> OnRemoveSettingAsync(string key, Storage.StorageType storageType)
        {
            return Task.Run<bool>(() =>
            {
                var store = GetSettingsContainer(storageType);

                if (store.Values.ContainsKey(key))
                {
                    return store.Values.Remove(key);
                }

                return false;
            });
        }

        /// <summary>
        /// Called when <see cref="M:SaveDataAsync" /> is executed to store an object in persistent storage.
        /// </summary>
        /// <param name="path">The string that identifies where the object will be stored.</param>
        /// <param name="data">The object that will be serialized and stored.</param>
        /// <param name="serializer">The implementation of <see cref="Wygwam.Windows.Storage.IDataSerializer" /> that will be used to serialize the specified object.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully persisted.
        /// </returns>
        protected override async Task<bool> OnSaveDataAsync(string path, object data, Wygwam.Windows.Storage.IDataSerializer serializer, Storage.StorageType storageType)
        {
            try
            {
                var file = await GetDataFolder(storageType).CreateFileAsync(path, _defaultCollisionPolicy);

                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    serializer.Serialize(fileStream, data);
                    await fileStream.FlushAsync();
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Called when <see cref="M:LoadDataAsync"/> is executed to load an object from persistent storage.
        /// </summary>
        /// <typeparam name="T">The type of the stored value.</typeparam>
        /// <param name="path">The string that identifies where the object is stored.</param>
        /// <param name="serializer">The implementation of <see cref="Wygwam.Windows.Storage.IDataSerializer"/> that will be used to deserialize the specified object.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully retrieved.
        /// </returns>
        protected override async Task<T> OnLoadDataAsync<T>(string path, Wygwam.Windows.Storage.IDataSerializer serializer, Storage.StorageType storageType)
        {
            try
            {
                var file = await GetDataFolder(storageType).GetFileAsync(path);

                using (var inStream = await file.OpenSequentialReadAsync())
                {
                    return serializer.Deserialize<T>(inStream.AsStreamForRead());
                }
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Called when <see cref="M:DeleteDataAsync" /> is executed to delete an object from persistent storage.
        /// </summary>
        /// <param name="path">The string that identifies where the object is stored.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully deleted.
        /// </returns>
        protected override async Task<bool> OnDeleteDataAsync(string path, Storage.StorageType storageType)
        {
            try
            {
                var file = await GetDataFolder(storageType).GetItemAsync(path);

                await file.DeleteAsync();

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Called when <see cref="M:GetDataFoldersAsync" /> is executed to retrieve a list of subfolders from persistent storage.
        /// </summary>
        /// <param name="path">The path of the parent folder.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        /// A list of the names of the subfolders contained in the requested folder.
        /// </returns>
        protected override async Task<IEnumerable<string>> OnGetDataFoldersAsync(string path, Storage.StorageType storageType)
        {
            var folder = await GetDataFolder(storageType).GetFolderAsync(path);

            return (await folder.GetFoldersAsync()).Select(f => f.Name);
        }

        /// <summary>
        /// Called when <see cref="M:GetDataFilesAsync" /> is executed to retrieve a list of file names from persistent storage.
        /// </summary>
        /// <param name="path">The path of the parent folder.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        /// A list of the names of the files contained in the requested folder.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override async Task<IEnumerable<string>> OnGetDataFilesAsync(string path, StorageType storageType)
        {
            var folder = await GetDataFolder(storageType).GetFolderAsync(path);

            return (await folder.GetFilesAsync()).Select(f => f.Name);
        }

        /// <summary>
        /// Gets the appropriate <see cref="global::Windows.Storage.StorageFolder"/> for the requested
        /// <see cref="Wygwam.Windows.Storage.StorageType"/>.
        /// </summary>
        /// <param name="storageType">The requested storage type.</param>
        /// <returns>The <see cref="global::Windows.Storage.StorageFolder"/> for the requested
        /// <see cref="Wygwam.Windows.Storage.StorageType"/>.</returns>
        private static StorageFolder GetDataFolder(Storage.StorageType storageType)
        {
            switch (storageType)
            {
                case Wygwam.Windows.Storage.StorageType.Roaming:
                    return ApplicationData.Current.RoamingFolder;
                case Wygwam.Windows.Storage.StorageType.Temp:
                    return ApplicationData.Current.TemporaryFolder;
                case Wygwam.Windows.Storage.StorageType.Local:
                default:
                    return ApplicationData.Current.LocalFolder;
            }
        }

        /// <summary>
        /// Gets the appropriate <see cref="global::Windows.Storage.ApplicationDataContainer"/> for the requested
        /// <see cref="Wygwam.Windows.Storage.StorageType"/>.
        /// </summary>
        /// <param name="storageType">The requested storage type.</param>
        /// <returns>The <see cref="global::Windows.Storage.ApplicationDataContainer"/> for the requested
        /// <see cref="Wygwam.Windows.Storage.StorageType"/>.</returns>
        private static ApplicationDataContainer GetSettingsContainer(Storage.StorageType storageType)
        {
            switch (storageType)
            {
                case Wygwam.Windows.Storage.StorageType.Roaming:
                    return ApplicationData.Current.RoamingSettings;
                case Wygwam.Windows.Storage.StorageType.Temp:
                case Wygwam.Windows.Storage.StorageType.Local:
                default:
                    return ApplicationData.Current.LocalSettings;
            }
        }
    }
}
