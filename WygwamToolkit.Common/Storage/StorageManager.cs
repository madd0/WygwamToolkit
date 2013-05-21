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

namespace Wygwam.Windows.Storage
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Wygwam.Windows;

    /// <summary>
    /// A base class to provide platform-independant access to a settings container and persistent storage.
    /// </summary>
    public abstract class StorageManager
    {
        private readonly IDataSerializer _defaultSerializer;

        private readonly AsyncLock _lock = new AsyncLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageManager"/> class.
        /// </summary>
        public StorageManager()
            : this(new XmlDataSerializer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageManager"/> class.
        /// </summary>
        /// <param name="defaultSerializer">The default serializer.</param>
        public StorageManager(IDataSerializer defaultSerializer)
        {
            _defaultSerializer = defaultSerializer;
        }

        /// <summary>
        /// Saves a key-value pair to a settings container.
        /// </summary>
        /// <param name="key">The string to use as a key for the element to save.</param>
        /// <param name="data">The object that will be saved.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the value was successfully stored.
        /// </returns>
        /// <remarks>
        ///   <para>Depending on the implementation, settings can be stored in isolated storage,
        /// the registry, etc.</para>
        ///   <para>The type of data that can be saved depends on the implementation, too.</para>
        /// </remarks>
        public async Task<bool> SaveSettingAsync(string key, object data, StorageType storageType = StorageType.Local)
        {
            using (var @lock = await _lock.LockAsync())
            {
                return await this.OnSaveSettingAsync(key, data, storageType);
            }
        }

        /// <summary>
        /// Loads a value stored in the settings container.
        /// </summary>
        /// <typeparam name="T">The type of the stored value.</typeparam>
        /// <param name="key">The string that was used as a key for the desired value.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>The value associated to the specified key.</returns>
        public async Task<T> LoadSettingAsync<T>(string key, StorageType storageType = StorageType.Local)
        {
            using (var @lock = await _lock.LockAsync())
            {
                return await this.OnLoadSettingAsync<T>(key, storageType);
            }
        }

        /// <summary>
        /// Deletes a value from the settings container.
        /// </summary>
        /// <param name="key">The string that identifies the value to delete.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the value was successfully deleted.
        /// </returns>
        public async Task<bool> RemoveSettingAsync(string key, StorageType storageType = StorageType.Local)
        {
            using (var @lock = await _lock.LockAsync())
            {
                return await this.OnRemoveSettingAsync(key, storageType);
            }
        }

        /// <summary>
        /// Saves a serialized object to persistent storage using the default XML serializer.
        /// </summary>
        /// <param name="path">The string that identifies where the object will be stored.</param>
        /// <param name="data">The object that will be serialized and stored.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully persisted.
        /// </returns>
        public Task<bool> SaveDataAsync(string path, object data, StorageType storageType = StorageType.Local)
        {
            return this.SaveDataAsync(path, data, _defaultSerializer, storageType);
        }

        /// <summary>
        /// Saves a serialized object to persistent storage using the specified serializer.
        /// </summary>
        /// <param name="path">The string that identifies where the object will be stored.</param>
        /// <param name="data">The object that will be serialized and stored.</param>
        /// <param name="serializer">The implementation of <see cref="IDataSerializer"/> that will be used to serialize the specified object.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully persisted.
        /// </returns>
        public async Task<bool> SaveDataAsync(string path, object data, IDataSerializer serializer, StorageType storageType = StorageType.Local)
        {
            using (var @lock = await _lock.LockAsync())
            {
                return await this.OnSaveDataAsync(path, data, serializer, storageType);
            }
        }

        /// <summary>
        /// Loads a serialized object from persistent storage using the default XML serializer.
        /// </summary>
        /// <typeparam name="T">The type of the stored value.</typeparam>
        /// <param name="path">The string that identifies where the object is stored.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully retrieved.
        /// </returns>
        public Task<T> LoadDataAsync<T>(string path, StorageType storageType = StorageType.Local)
        {
            return this.LoadDataAsync<T>(path, _defaultSerializer, storageType);
        }

        /// <summary>
        /// Loads a serialized object from persistent storage using the specified serializer.
        /// </summary>
        /// <typeparam name="T">The type of the stored value.</typeparam>
        /// <param name="path">The string that identifies where the object is stored.</param>
        /// <param name="serializer">The implementation of <see cref="IDataSerializer"/> that will be used to deserialize the specified object.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully retrieved.
        /// </returns>
        public async Task<T> LoadDataAsync<T>(string path, IDataSerializer serializer, StorageType storageType = StorageType.Local)
        {
            using (var @lock = await _lock.LockAsync())
            {
                return await this.OnLoadDataAsync<T>(path, serializer, storageType);
            }
        }

        /// <summary>
        /// Deletes an object from persistent storage.
        /// </summary>
        /// <param name="path">The string that identifies where the object is stored.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully deleted.
        /// </returns>
        public async Task<bool> DeleteDataAsync(string path, StorageType storageType = StorageType.Local)
        {
            using (var @lock = await _lock.LockAsync())
            {
                return await this.OnDeleteDataAsync(path, storageType);
            }
        }

        /// <summary>
        /// Gets a list of folder names contained in the desired path.
        /// </summary>
        /// <param name="path">The path of the parent folder.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>A list of the names of the subfolders contained in the requested folder.</returns>
        public async Task<IEnumerable<string>> GetDataFoldersAsync(string path, StorageType storageType = StorageType.Local)
        {
            using (var @lock = await _lock.LockAsync())
            {
                return await this.OnGetDataFoldersAsync(path, storageType);
            }
        }

        /// <summary>
        /// Gets a list of file names contained in the desired path.
        /// </summary>
        /// <param name="path">The path of the parent folder.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>A list of the names of the subfolders contained in the requested folder.</returns>
        public async Task<IEnumerable<string>> GetDataFilesAsync(string path, StorageType storageType = StorageType.Local)
        {
            using (var @lock = await _lock.LockAsync())
            {
                return await this.OnGetDataFilesAsync(path, storageType);
            }
        }

        /// <summary>
        /// Called when <see cref="M:SaveSettingAsync"/> is executed to store data in the settings container.
        /// </summary>
        /// <param name="key">The string to use as a key for the element to save.</param>
        /// <param name="data">The object that will be saved.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the value was successfully stored.
        /// </returns>
        protected abstract Task<bool> OnSaveSettingAsync(string key, object data, StorageType storageType);

        /// <summary>
        /// Called when <see cref="M:LoadSettingAsync{T}"/> is executed to load data from the settings container.
        /// </summary>
        /// <typeparam name="T">The type of the stored value.</typeparam>
        /// <param name="key">The string that was used as a key for the desired value.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>The value associated to the specified key.</returns>
        protected abstract Task<T> OnLoadSettingAsync<T>(string key, StorageType storageType);

        /// <summary>
        /// Called when <see cref="M:RemoveSettingAsync"/> is executed to delete a value from the settings container.
        /// </summary>
        /// <param name="key">The string that identifies the value to delete.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the value was successfully deleted.
        /// </returns>
        protected abstract Task<bool> OnRemoveSettingAsync(string key, StorageType storageType);

        /// <summary>
        /// Called when <see cref="M:SaveDataAsync" /> is executed to store an object in persistent storage.
        /// </summary>
        /// <param name="path">The string that identifies where the object will be stored.</param>
        /// <param name="data">The object that will be serialized and stored.</param>
        /// <param name="serializer">The implementation of <see cref="IDataSerializer" /> that will be used to serialize the specified object.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully persisted.
        /// </returns>
        protected abstract Task<bool> OnSaveDataAsync(string path, object data, IDataSerializer serializer, StorageType storageType);

        /// <summary>
        /// Called when <see cref="M:LoadDataAsync"/> is executed to load an object from persistent storage.
        /// </summary>
        /// <typeparam name="T">The type of the stored value.</typeparam>
        /// <param name="path">The string that identifies where the object is stored.</param>
        /// <param name="serializer">The implementation of <see cref="IDataSerializer"/> that will be used to deserialize the specified object.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully retrieved.
        /// </returns>
        protected abstract Task<T> OnLoadDataAsync<T>(string path, IDataSerializer serializer, StorageType storageType);

        /// <summary>
        /// Called when <see cref="M:DeleteDataAsync"/> is executed to delete an object from persistent storage.
        /// </summary>
        /// <param name="path">The string that identifies where the object is stored.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>
        ///   <c>true</c> if the object was successfully deleted.
        /// </returns>
        protected abstract Task<bool> OnDeleteDataAsync(string path, StorageType storageType);

        /// <summary>
        /// Called when <see cref="M:GetDataFoldersAsync"/> is executed to retrieve a list of subfolders from persistent storage.
        /// </summary>
        /// <param name="path">The path of the parent folder.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>A list of the names of the subfolders contained in the requested folder.</returns>
        protected abstract Task<IEnumerable<string>> OnGetDataFoldersAsync(string path, StorageType storageType);

        /// <summary>
        /// Called when <see cref="M:GetDataFilesAsync"/> is executed to retrieve a list of file names from persistent storage.
        /// </summary>
        /// <param name="path">The path of the parent folder.</param>
        /// <param name="storageType">Defines the desired storage type. Not all implementations support all storage types.</param>
        /// <returns>A list of the names of the files contained in the requested folder.</returns>
        protected abstract Task<IEnumerable<string>> OnGetDataFilesAsync(string path, StorageType storageType);
    }
}
