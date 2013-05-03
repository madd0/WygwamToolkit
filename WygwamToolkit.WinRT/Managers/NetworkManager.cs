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

using Windows.Networking.Connectivity;
using Wygwam.Windows.Networking;

namespace Wygwam.Windows.WinRT
{
    /// <summary>
    /// Provide access to network connection information.
    /// </summary>
    public class NetworkManager : Wygwam.Windows.Networking.NetworkManager
    {
        private bool _usedCachedProfile;

        private ConnectionProfile _cachedProfile;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkManager"/> class.
        /// </summary>
        public NetworkManager()
        {
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
        }

        /// <summary>
        /// Gets a value indicating whether a network connection is available.
        /// </summary>
        public override bool IsNetworkAvailable
        {
            get
            {
                return this.CurrentInternetConnectionProfile != null &&
                    this.CurrentInternetConnectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            }
        }

        /// <summary>
        /// Gets the type of the connection.
        /// </summary>
        public override InternetConnectionType ConnectionType
        {
            get
            {
                if (!this.IsNetworkAvailable)
                {
                    return InternetConnectionType.None;
                }

                switch (this.CurrentInternetConnectionProfile.NetworkAdapter.IanaInterfaceType)
                {
                    case 6:
                        return InternetConnectionType.Ethernet;
                    case 71:
                        return InternetConnectionType.WiFi;
                    case 243:
                        return InternetConnectionType.Cell;
                }

                return InternetConnectionType.Unknown;
            }
        }

        /// <summary>
        /// Gets the current Internet connection profile.
        /// </summary>
        /// <remarks>Can use a cached version.</remarks>
        private ConnectionProfile CurrentInternetConnectionProfile
        {
            get
            {
                return _usedCachedProfile ? _cachedProfile : NetworkInformation.GetInternetConnectionProfile();
            }
        }

        /// <summary>
        /// Gets the current network status.
        /// </summary>
        /// <param name="sender">The object notifying that network status changed.</param>
        /// <returns>
        /// An instance of <see cref="NetworkChangedEventArgs" /> describing the current network status.
        /// </returns>
        protected override NetworkChangedEventArgs GetCurrentNetworkStatus(object sender)
        {
            try
            {
                _cachedProfile = NetworkInformation.GetInternetConnectionProfile();
                _usedCachedProfile = true;

                return new NetworkChangedEventArgs(this.IsNetworkAvailable, this.ConnectionType);
            }
            finally
            {
                _cachedProfile = null;
                _usedCachedProfile = false;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
            }
        }
    }
}
