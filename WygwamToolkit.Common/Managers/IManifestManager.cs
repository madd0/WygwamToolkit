namespace Wygwam.Windows.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Managed the manifest Data
    /// </summary>
    public interface IManifestManager : IReadOnlyDictionary<string, string>
    {
        #region Properties

        /// <summary>
        /// Gets the display name of the publisher.
        /// </summary>
        string PublisherDisplayName { get; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the logo uri.
        /// </summary>
        Uri Logo { get; }

        /// <summary>
        /// Gets the splash screen image.
        /// </summary>
        Uri SplashScreenImage { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the Produt ID.
        /// </summary>
        string ProductId { get; }

        /// <summary>
        /// Gets the publisher.
        /// </summary>
        string PublisherID { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        Version Version { get; }

        #endregion

        Task<IManifestManager> Load();
        Task<IManifestManager> Load(string filename);
        Task<IManifestManager> Load(Stream data);
    }
}
