using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WygwamToolkit.Common.Managers
{
    public abstract class ANetworkManager
    {
        public event EventHandler<NetworkChangedEventArgs> InternetConnectionChanged;

        public abstract bool IsNetworkAvailable { get; }

        protected void NetworkInformation_NetworkStatusChanged(object sender)
        {
            var arg = new NetworkChangedEventArgs { IsConnected = this.IsNetworkAvailable };

            if (InternetConnectionChanged != null)
                InternetConnectionChanged(null, arg);
        }
    }

    public class NetworkChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
    }
}
