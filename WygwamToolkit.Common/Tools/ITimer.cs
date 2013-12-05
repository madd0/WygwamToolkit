//-----------------------------------------------------------------------
// <copyright file="ITimer.cs" company="Wygwam">
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

namespace Wygwam.Windows
{
    using System;

    /// <summary>
    /// Provides and interface that abstracts the behavior of a DispatcherTimer.
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// Occurs when the timer interval has elapsed.
        /// </summary>
        event EventHandler Tick;

        /// <summary>
        /// Gets or sets the amount of time between timer ticks.
        /// </summary>
        /// <value>
        /// The amount of time between ticks. The default is a <see cref="System.TimeSpan"/> 
        /// with value evaluated as 00:00:00.
        /// </value>
        TimeSpan Interval { get; set; }

        /// <summary>
        /// Gets a value indicating whether the wrapped timer is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if the timer is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; }

        /// <summary>
        /// Starts the wrapped timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the wrapped timer.
        /// </summary>
        void Stop();
    }
}
