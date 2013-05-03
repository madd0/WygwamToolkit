using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wygwam.Windows;

namespace WygwamToolkit.Common.Managers
{
    public abstract class AStorageManager
    {
        private readonly AsyncSemaphore _lock = new AsyncSemaphore(1);

        #region Settings
        public async Task<bool> SaveSettingAsync(string key, object data)
        {
            bool ret = false;

            await _lock.WaitAsync();
            try
            {
                ret = await this.SavePlatformAsync(key, data);
            }
            finally { _lock.Release(); }
            return ret;
        }

        public async Task<T> LoadSettingAsync<T>(string key)
        {
            T ret;

            await _lock.WaitAsync();
            try
            {
                ret = await this.LoadPlatformAsync<T>(key);
            }
            finally { _lock.Release(); }

            return ret;
        }

        public async Task<bool> ClearSettingAsync(string key)
        {
            bool ret;

            await _lock.WaitAsync();
            try
            {
                ret = await this.ClearPlatformAsync(key);
            }
            finally { _lock.Release(); }

            return ret;
        }

        #endregion

        #region Data
        public async Task<bool> SaveDataAsync(string path, object data)
        {
            bool ret;

            await _lock.WaitAsync();
            try
            {
                ret = await this.SaveDataPlatformAsync(path, data);
            }
            finally { _lock.Release(); }

            return ret;
        }

        public async Task<T> LoadDataAsync<T>(string path)
        {
            T ret;

            await _lock.WaitAsync();
            try
            {
                ret = await this.LoadDataPlatformAsync<T>(path);
            }
            finally { _lock.Release(); }

            return ret;
        }

        public async Task<bool> ClearDataAsync(string path)
        {
            bool ret;

            await _lock.WaitAsync();
            try
            {
                ret = await this.ClearDataPlatformAsync(path);
            }
            finally { _lock.Release(); }

            return ret;
        }

        #endregion

        #region virtual
        protected abstract Task<bool> SavePlatformAsync(string key, object data);

        protected abstract Task<T> LoadPlatformAsync<T>(string key);

        protected abstract Task<bool> ClearPlatformAsync(string key);


        protected abstract Task<bool> SaveDataPlatformAsync(string path, object data);

        protected abstract Task<T> LoadDataPlatformAsync<T>(string path);

        protected abstract Task<bool> ClearDataPlatformAsync(string path);
        #endregion
    }
}
