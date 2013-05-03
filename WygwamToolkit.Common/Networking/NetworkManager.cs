//-----------------------------------------------------------------------
// <copyright file="NetworkManager.cs" company="Wygwam">
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

using System;

namespace Wygwam.Windows.Networking
{
    /// <summary>
    /// Serves as a base for platform-specific classes that provide access to network connection information.
    /// </summary>
    public abstract class NetworkManager : IDisposable
    {
        /// <summary>
        /// Occurs when the Internet connection changes.
        /// </summary>
        public event EventHandler<NetworkChangedEventArgs> InternetConnectionChanged;

        /// <summary>
        /// Gets a value indicating whether a network connection is available.
        /// </summary>
        public abstract bool IsNetworkAvailable { get; }

        /// <summary>
        /// Gets the type of the connection.
        /// </summary>
        public abstract InternetConnectionType ConnectionType { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the current network status.
        /// </summary>
        /// <param name="sender">The object notifying that network status changed.</param>
        /// <returns>An instance of <see cref="NetworkChangedEventArgs"/> describing the current network status.</returns>
        protected abstract NetworkChangedEventArgs GetCurrentNetworkStatus(object sender);

        /// <summary>
        /// Called when network status changes.
        /// </summary>
        /// <param name="sender">The object notifying that network status changed.</param>
        protected virtual void OnNetworkStatusChanged(object sender)
        {
            this.OnInternetConnectionChanged(this.GetCurrentNetworkStatus(sender));
        }

        /// <summary>
        /// Raises the <see cref="E:NetworkManager.InternetConnectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:NetworkChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnInternetConnectionChanged(NetworkChangedEventArgs e)
        {
            EventHandler<NetworkChangedEventArgs> handler = this.InternetConnectionChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
