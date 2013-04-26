using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using WygwamToolkit.Common.Managers;

namespace WygwamToolkit.WinRT.Managers
{
    public class LocationManager : ILocationManager
    {
        public GeoPosition LastKnowPosition { get; set; }

        public async Task<GeoPosition> GetLocationAsync()
        {
            Geolocator gl = new Geolocator();
            Geoposition gp = await gl.GetGeopositionAsync();

            return new GeoPosition(gp.Coordinate.Latitude, gp.Coordinate.Longitude);
        }

        public Task<bool> AskForRightAsync()
        {
            throw new NotImplementedException();
        }
    }
}
