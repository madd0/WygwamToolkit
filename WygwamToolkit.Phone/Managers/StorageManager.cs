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

namespace Wygwam.Windows.Phone
{
    using global::Windows.Storage;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
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

                if (store.Contains(key))
                {
                    store[key] = data;
                }
                else
                {
                    store.Add(key, data);
                }

                store.Save();

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

                if (!store.Contains(key))
                {
                    return default(T);
                }

                return (T)store[key];
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

                if (store.Contains(key))
                {
                    return store.Remove(key);
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
                var file = await (await GetDataFolder(storageType, path)).CreateFileAsync(Path.GetFileName(path), _defaultCollisionPolicy);

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
                var file = await (await GetDataFolder(storageType, path)).GetFileAsync(Path.GetFileName(path));

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
                var file = await (await GetDataFolder(storageType, path)).GetItemAsync(Path.GetFileName(path));

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
            var folder = await (await GetDataFolder(storageType, path)).GetFolderAsync(Path.GetFileName(path));

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
            var folder = await (await GetDataFolder(storageType, path)).GetFolderAsync(Path.GetFileName(path));

            return (await folder.GetFilesAsync()).Select(f => f.Name);
        }

        /// <summary>
        /// Gets the appropriate <see cref="global::Windows.Storage.StorageFolder"/> for the requested
        /// <see cref="Wygwam.Windows.Storage.StorageType"/>.
        /// </summary>
        /// <param name="storageType">The requested storage type.</param>
        /// <returns>The <see cref="global::Windows.Storage.StorageFolder"/> for the requested
        /// <see cref="Wygwam.Windows.Storage.StorageType"/>.</returns>
        private static async Task<StorageFolder> GetDataFolder(Storage.StorageType storageType, string path)
        {
            var directory = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directory) || directory.Equals("\\") || directory.Equals("/") || directory.Equals("//"))
                return ApplicationData.Current.LocalFolder;

            StorageFolder folder = await GetDataFolder(storageType, directory);

            var folders = await folder.GetFoldersAsync();

            string folderName = Path.GetFileName(directory);
            var tmpFolder = (from f in folders
                      where string.Equals(f.Name, folderName, StringComparison.InvariantCultureIgnoreCase)
                      select f).FirstOrDefault();

            if (tmpFolder == null)
               tmpFolder = await folder.CreateFolderAsync(folderName);

            return tmpFolder;
        }

        /// <summary>
        /// Gets the appropriate <see cref="global::Windows.Storage.ApplicationDataContainer"/> for the requested
        /// <see cref="Wygwam.Windows.Storage.StorageType"/>.
        /// </summary>
        /// <param name="storageType">The requested storage type.</param>
        /// <returns>The <see cref="global::Windows.Storage.ApplicationDataContainer"/> for the requested
        /// <see cref="Wygwam.Windows.Storage.StorageType"/>.</returns>
        private static IsolatedStorageSettings GetSettingsContainer(Storage.StorageType storageType)
        {
            return IsolatedStorageSettings.ApplicationSettings;
        }

    }
}
