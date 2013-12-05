using System;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using Wygwam.Windows.Location;

namespace WygwamToolkit.Phone.Managers
{
    public class LocationManager : ILocationManager
    {
        public GeoPosition LastKnowPosition { get; set; }

        public Task<bool> AskForRightAsync()
        {
            MessageBoxResult result =
            MessageBox.Show("L'application a besoin de connaitre votre position, êtes vous d'accord?",
            "Position",
            MessageBoxButton.OKCancel);

            return Task.FromResult<bool>(result == MessageBoxResult.OK);
        }

        public async Task<GeoPosition> GetLocationAsync()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            Geoposition geoposition = await geolocator.GetGeopositionAsync(
             maximumAge: TimeSpan.FromMinutes(5),
             timeout: TimeSpan.FromSeconds(10)
             );

            if (geoposition == null)
                return null;

            this.LastKnowPosition = new GeoPosition(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);

            return this.LastKnowPosition;
        }
    }
}
