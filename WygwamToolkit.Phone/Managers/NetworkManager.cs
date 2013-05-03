using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using WygwamToolkit.Common.Managers;

namespace WygwamToolkit.Phone.Managers
{
    public class NetworkManager : Wygwam.Windows.Networking.NetworkManager
    {
        public override bool IsNetworkAvailable
        {
            get
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
        }

        public NetworkManager()
        {
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
        }

        protected override Wygwam.Windows.Networking.NetworkChangedEventArgs GetCurrentNetworkStatus(object sender)
        {
            throw new NotImplementedException();
        }
    }
}
