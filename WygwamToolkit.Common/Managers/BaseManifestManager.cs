namespace Wygwam.Windows.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Managed the manifest Data
    /// </summary>
    public abstract class BaseManifestManager : IManifestManager
    {
        #region Fields
        
        private static IManifestManager _current;

        private static Task<IManifestManager> _currentLoading;

        protected readonly Dictionary<string, string> _manifestInformation;

        #endregion

        #region ctor

        public BaseManifestManager()
        {
            _manifestInformation = new Dictionary<string, string>();
        }

        #endregion

        #region Properties

        public static IManifestManager Current
        {
            get
            {
                if (_currentLoading != null && !_currentLoading.IsCompleted)
                    Task.WaitAll(_currentLoading);

                return _current;
            }
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        public IEnumerable<string> Keys
        {
            get { return this._manifestInformation.Keys; }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IEnumerable<string> Values
        {
            get { return this._manifestInformation.Values; }
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return this._manifestInformation[key]; }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get { return _manifestInformation.Count; }
        }

        /// <summary>
        /// Gets the display name of the publisher.
        /// </summary>
        public string PublisherDisplayName { get; protected set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName { get; protected set; }

        /// <summary>
        /// Gets the logo uri.
        /// </summary>
        public Uri Logo { get; protected set; }

        /// <summary>
        /// Gets the splash screen image.
        /// </summary>
        public Uri SplashScreenImage { get; protected set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Gets the Produt ID.
        /// </summary>
        public string ProductId { get; protected set; }

        /// <summary>
        /// Gets the publisher ID.
        /// </summary>
        public string PublisherID { get; protected set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public Version Version { get; protected set; }

        #endregion

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public Task<IManifestManager> Load()
        {
            if (_current != null)
                return Task.FromResult(_current);

            if (_currentLoading != null)
                return _currentLoading;

            _currentLoading = LoadDefaultManifest();
            return _currentLoading;
        }

        /// <summary>
        /// Loads the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public abstract Task<IManifestManager> Load(string filename);

        /// <summary>
        /// Loads the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public abstract Task<IManifestManager> Load(System.IO.Stream data);

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return this._manifestInformation.ContainsKey(key);
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out string value)
        {
            return this._manifestInformation.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this._manifestInformation.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._manifestInformation.GetEnumerator();
        }

        /// <summary>
        /// Loads the default manifest.
        /// </summary>
        /// <returns></returns>
        private async Task<IManifestManager> LoadDefaultManifest()
        {
            _current = await OnLoading();
            return _current;
        }

        protected virtual Task<IManifestManager> OnLoading()
        {
            return Task.FromResult<IManifestManager>(null);
        }
    }
}

