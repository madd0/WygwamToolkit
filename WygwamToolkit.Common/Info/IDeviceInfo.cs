//-----------------------------------------------------------------------
// <copyright file="DeviceInfo.cs" company="Wygwam">
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

namespace Wygwam.Windows.Info
{
    using System;

    /// <summary>
    /// Instance grouping many information for the device
    /// </summary>
    public interface IDeviceInfo
    {
        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the OS name.
        /// </summary>
        string OS { get; }

        /// <summary>
        /// Gets the OS's version.
        /// </summary>
        Version OSVersion { get; }

        /// <summary>
        /// Gets the manufaturer name.
        /// </summary>
        string Manufaturer { get; }

        /// <summary>
        /// Gets the device version.
        /// </summary>
        Version DeviceVersion { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        string Model { get; }
    }
}
