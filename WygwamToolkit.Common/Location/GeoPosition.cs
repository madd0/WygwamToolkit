//-----------------------------------------------------------------------
// <copyright file="GeoPosition.cs" company="Wygwam">
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

namespace Wygwam.Windows.Location
{
    using System;

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
