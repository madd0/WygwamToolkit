using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wygwam.Windows.Phone
{
    public class NetworkManager : Networking.NetworkManager
    {
        #region Fields
        
        private Timer _timer;
        private string _lastcellularOperator;
        private Networking.InternetConnectionType _lastConnectionType;

        #endregion

        #region CTOR

        public NetworkManager()
        {
            Microsoft.Phone.Net.NetworkInformation.DeviceNetworkInformation.NetworkAvailabilityChanged += DeviceNetworkInformation_NetworkAvailabilityChanged;
            //_lastcellularOperator = DeviceNetworkInformation.CellularMobileOperator;
            //_lastConnectionType = ConnectionType;

            //if (SmartDispatcher.IsInitialized)
            //    InitializeTimer(null, null);
            //else
            //    SmartDispatcher.Initialized += InitializeTimer;
        }

        #endregion

        public override bool IsNetworkAvailable
        {
            get { return NetworkInterface.GetIsNetworkAvailable(); }
        }

        public override Networking.InternetConnectionType ConnectionType
        {
            get 
            {
                if (!DeviceNetworkInformation.IsNetworkAvailable)
                    return Networking.InternetConnectionType.None;

                if (DeviceNetworkInformation.IsWiFiEnabled)
                    return Networking.InternetConnectionType.WiFi;
                if (DeviceNetworkInformation.IsCellularDataEnabled)
                    return Networking.InternetConnectionType.Cell;

                return Networking.InternetConnectionType.Unknown;
            }
        }

        protected override Networking.NetworkChangedEventArgs GetCurrentNetworkStatus(object sender)
        {
            return new Networking.NetworkChangedEventArgs(IsNetworkAvailable, ConnectionType);
        }
        
        private void DeviceNetworkInformation_NetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            //_timer.Stop();
            try
            {
                OnNetworkStatusChanged(e);
                _lastcellularOperator = DeviceNetworkInformation.CellularMobileOperator;
                _lastConnectionType = ConnectionType;
            }
            catch
            {

            }

            //_timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_lastConnectionType != ConnectionType)
                OnNetworkStatusChanged(null);

            if (_lastcellularOperator != DeviceNetworkInformation.CellularMobileOperator)
                OnNetworkStatusChanged(null);
        }

        private void InitializeTimer(object sender, EventArgs e)
        {
            SmartDispatcher.Initialized -= InitializeTimer;
            _timer = new Timer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();
        }
    }
}
