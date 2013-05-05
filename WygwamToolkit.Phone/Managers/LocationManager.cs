using System;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using WygwamToolkit.Common.Managers;

namespace WygwamToolkit.Phone.Managers
{
    public class LocationManager : ILocationManager
    {
        public GeoPosition LastKnowPosition { get; set; }

        public async Task<bool> AskForRightAsync()
        {
            MessageBoxResult result =
            MessageBox.Show("L'application a besoin de connaitre votre position, êtes vous d'accord?",
            "Position",
            MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
                return true;
            else
                return false;
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
