namespace Wygwam.Windows.Phone.Security
{
    using global::Windows.Phone.System.Analytics;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization.Json;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Wygwam.Windows.Phone.Managers;
    using Wygwam.Windows.Security;
    using Wygwam.Windows.Tools;

    /// <summary>
    /// Managed credential with secured password
    /// </summary>
    /// <remarks>This class needed the capacity ID_CAP_IDENTITY_DEVICE</remarks>
    public sealed class SecurePassword<T> : ISecurePassword
            where T : Credential, new()
    {
        #region Fields

        private readonly static string fileKey;
        private readonly static string fileName;

        private static bool _isSaving;
        private TaskCompletionSource<bool> _waitingTask;
        private static AsyncAction _waitingSave;

        /// <summary>
        /// The key
        /// </summary>
        private static readonly byte[] key;

        private List<Credential> _credentials;

        #endregion

        #region CTOR

        /// <summary>
        /// Initializes the <see cref="SecurePassword"/> class.
        /// </summary>
        /// <remarks>This class needed the capacity ID_CAP_IDENTITY_DEVICE</remarks>
        static SecurePassword()
        {
            var keyString = HostInformation.PublisherHostId;
            string productId = null;

            ManifestManager mgr = new ManifestManager();
            mgr.Load();

            var result = ManifestManager.Current.TryGetValue("ProductID", out productId);
            Debug.Assert(result, "The product ID is missing in the manifest");

            var keyBytes = Encoding.UTF8.GetBytes(keyString + "#" + productId);
            var baseString = Convert.ToBase64String(keyBytes);
            key = Encoding.UTF8.GetBytes(baseString);

            fileKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyString + typeof(T).ToString()));
            fileName = productId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurePassword"/> class.
        /// </summary>
        public SecurePassword()
        {
            _credentials = new List<Credential>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public int Count
        {
            get { return this._credentials.Count; }
        }

        #endregion

        /// <summary>
        /// Adds the specified credential.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Add(Credential credential)
        {
            if (!_credentials.Contains(credential))
            {
                _credentials.Add(credential);
                Save();
            }
        }

        /// <summary>
        /// Removes the specified credential.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Remove(Credential credential)
        {
            if (_credentials.Contains(credential))
            {
                _credentials.Remove(credential);
                Save();
            }
        }

        /// <summary>
        /// Gets all credentials by token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<Credential> GetAllByToken(string token)
        {
            return (from el in _credentials
                    where string.Equals(el.Token, token, StringComparison.InvariantCultureIgnoreCase)
                    select el);
        }

        /// <summary>
        /// Gets all credentials by user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<Credential> GetAllByUserName(string userName)
        {
            return (from el in _credentials
                    where string.Equals(el.Username, userName, StringComparison.InvariantCultureIgnoreCase)
                    select el);
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task Load()
        {
            try
            {
                using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.FileExists(fileName))
                        return;

                    var file = store.OpenFile(fileName, System.IO.FileMode.Open);

                    byte[] data = new byte[file.Length];
                    var read = await file.ReadAsync(data, 0, (int)file.Length);

                    var length = file.Length;
                    file.Dispose();
                    file = null;

                    if (read != length)
                        return;

                    var offset = store.GetCreationTime(fileName);

                    data = ProtectedData.Unprotect(data, GetFileAccessKey(offset));

                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T[]));

                    Credential[] credentials = null;

                    using (MemoryStream stream = new MemoryStream(data))
                    {
                        credentials = (T[])json.ReadObject(stream);
                    }

                    foreach (var c in credentials)
                    {
                        var props = c.GetType().GetProperties();

                        foreach (var p in props)
                        {
                            if (p.GetCustomAttribute(typeof(SecurityPropertyAttribute)) != null)
                            {
                                var str = (string)p.GetValue(c);
                                if (string.IsNullOrEmpty(str))
                                    continue;
                                var strBytes = Convert.FromBase64String(str);
                                var unProtectedData = ProtectedData.Unprotect(strBytes, key);
                                p.SetValue(c, Encoding.UTF8.GetString(unProtectedData, 0, unProtectedData.Length));
                            }

                        }
                    }

                    _credentials.AddRange(credentials);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        /// <returns></returns>
        public async Task Commit()
        {
            await Save();
        }


        private async Task Save()
        {
            if (_isSaving)
            {
                if (_waitingSave == null)
                {
                    if (_waitingTask != null)
                        _waitingTask.TrySetResult(true);

                    _waitingTask = new TaskCompletionSource<bool>();
                    _waitingSave = async () =>
                        {
                            await Save();
                            _waitingTask.SetResult(true);
                        };
                }
                await _waitingTask.Task;
                return;
            }

            _isSaving = true;

            try
            {
                if (_credentials.Count > 0)
                {
                    using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        var file = store.OpenFile(fileName, System.IO.FileMode.Create);
                        var offset = store.GetCreationTime(fileName);

                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T[]));
                        using (MemoryStream stream = new MemoryStream())
                        {
                            List<T> toSave = new List<T>();
                            foreach (var cre in _credentials)
                            {
                                T nCre = new T();

                                foreach (var p in cre.GetType().GetProperties())
                                {
                                    if (p.GetCustomAttribute(typeof(SecurityPropertyAttribute)) != null)
                                    {
                                        var str = (string)p.GetValue(cre);
                                        if (string.IsNullOrEmpty(str))
                                            continue;
                                        var data = ProtectedData.Protect(Encoding.UTF8.GetBytes(str), key);
                                        p.SetValue(nCre, Convert.ToBase64String(data, 0, data.Length));
                                    }
                                    else
                                       p.SetValue(nCre, p.GetValue(cre));
                                }
                                toSave.Add(nCre);
                            }

                            serializer.WriteObject(stream, toSave.ToArray());

                            stream.Seek(0, SeekOrigin.Begin);

                            byte[] allData = new byte[stream.Length];
                            await stream.ReadAsync(allData, 0, (int)stream.Length);

                            allData = ProtectedData.Protect(allData, GetFileAccessKey(offset));

                            await file.WriteAsync(allData, 0, allData.Length);

                            file.Dispose();
                            file = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            _isSaving = false;

            if (_waitingSave != null)
            {
                var act = _waitingSave;
                _waitingSave = null;
                act();
            }
        }

        private byte[] GetFileAccessKey(DateTimeOffset offset)
        {
            return Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(key, 0, key.Length) + fileKey + offset.Ticks)));
        }
    }
}
