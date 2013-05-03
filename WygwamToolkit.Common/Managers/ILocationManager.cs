using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WygwamToolkit.Common.Managers
{
    public interface ILocationManager
    {
        GeoPosition LastKnowPosition { get; set; }

        Task<GeoPosition> GetLocationAsync();
        Task<bool> AskForRightAsync();
    }

    public class GeoPosition
    {
        public enum Unit
        {
            Km,
            Miles,
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoPosition(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public double DistanceTo(GeoPosition pos, Unit unit = Unit.Km)
        {
            var rlat1 = Math.PI * this.Latitude / 180;
            var rlat2 = Math.PI * pos.Latitude / 180;
            var rlon1 = Math.PI * this.Longitude / 180;
            var rlon2 = Math.PI * pos.Longitude / 180;
            var theta = this.Longitude - pos.Longitude;
            var rtheta = Math.PI * theta / 180;
            var dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            if (unit == Unit.Km) { dist = dist * 1.609344; }
            if (unit == Unit.Miles) { dist = dist * 0.8684; }
            return dist;
        }
    }
}
