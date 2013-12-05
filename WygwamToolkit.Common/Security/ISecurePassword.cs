//-----------------------------------------------------------------------
// <copyright file="ISecurePassword.cs" company="Wygwam">
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

namespace Wygwam.Windows.Security
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Managed credential with secured password
    /// </summary>
    public interface ISecurePassword
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        int Count { get; }

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
    }
}
