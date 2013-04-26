using Windows.Networking.Connectivity;
using WygwamToolkit.Common.Managers;

namespace PMV.Windows8.Common.Managers
{
    public class NetworkManager : ANetworkManager
    {
        public override bool IsNetworkAvailable
        {
            get
            {
                return NetworkInformation.GetInternetConnectionProfile() != null;
            }
        }

        public NetworkManager()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
        }
    }
}
