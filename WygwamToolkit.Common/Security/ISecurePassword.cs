namespace Wygwam.Windows.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Managed credential with secured password
    /// </summary>
    public interface ISecurePassword
    {
        #region Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        int Count { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified credential.
        /// </summary>
        /// <param name="credential">The credential.</param>
        void Add(Credential credential);
        /// <summary>
        /// Removes the specified credential.
        /// </summary>
        /// <param name="credential">The credential.</param>
        void Remove(Credential credential);

        /// <summary>
        /// Gets all credentials by token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        IEnumerable<Credential> GetAllByToken(string token);

        /// <summary>
        /// Gets all credentials by user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        IEnumerable<Credential> GetAllByUserName(string userName);

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        Task Load();

        /// <summary>
        /// Commits this instance.
        /// </summary>
        /// <returns></returns>
        Task Commit();

        #endregion
    }
}
