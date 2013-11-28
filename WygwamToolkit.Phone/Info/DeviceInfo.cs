namespace Wygwam.Windows.Phone.Info
{
    using Microsoft.Phone.Info;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Wygwam.Windows.Info;

    /// <summary>
    /// Aggregate many information on the device
    /// </summary>
    public class DeviceInfo : IDeviceInfo
    {
        #region Fields

        private static readonly DeviceInfo _current = new DeviceInfo();

        #endregion

        #region CTOR

        /// <summary>
        /// Prevents a default instance of the <see cref="DeviceInfo"/> class from being created.
        /// </summary>
        private DeviceInfo()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton instance of the deviceinfo class
        /// </summary>
        public static DeviceInfo Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        public string Name
        {
            get { return global::Windows.Networking.Proximity.PeerFinder.DisplayName; }
        }

        /// <summary>
        /// Gets the OS name.
        /// </summary>
        public string OS
        {
            get { return Enum.GetName(typeof(PlatformID), Environment.OSVersion.Platform); }
        }

        /// <summary>
        /// Gets the OS's version.
        /// </summary>
        public Version OSVersion
        {
            get { return Environment.OSVersion.Version; }
        }

        /// <summary>
        /// Gets the manufaturer name.
        /// </summary>
        public string Manufaturer
        {
            get { return DeviceStatus.DeviceManufacturer; }
        }

        /// <summary>
        /// Gets the device version.
        /// </summary>
        public Version DeviceVersion
        {
            get
            {
                Version v = null;
                if (!Version.TryParse(DeviceStatus.DeviceHardwareVersion, out v))
                {
                    v = new Version(0, 0);
                }
                return v;
            }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public string Model
        {
            get { return DeviceStatus.DeviceName; }
        }
        #endregion
    }
}
