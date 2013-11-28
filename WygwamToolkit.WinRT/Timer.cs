//-----------------------------------------------------------------------
// <copyright file="Timer.cs" company="Wygwam">
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
namespace Wygwam.Windows
{
    using System;

    using global::Windows.UI.Xaml;

    /// <summary>
    /// Wraps the <see cref="global::Windows.UI.Xaml.DispatcherTimer"/> to make it accessible
    /// via the <see cref="ITimer"/> interface.
    /// </summary>
    public class Timer : ITimer
    {
        private DispatcherTimer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        public Timer()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
        }

        /// <summary>
        /// Occurs when the timer interval has elapsed.
        /// </summary>
        public event EventHandler Tick;

        /// <summary>
        /// Gets or sets the amount of time between timer ticks.
        /// </summary>
        /// <value>
        /// The amount of time between ticks. The default is a <see cref="System.TimeSpan" />
        /// with value evaluated as 00:00:00.
        /// </value>
        public TimeSpan Interval
        {
            get { return _timer.Interval; }

            set { _timer.Interval = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the wrapped timer is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the timer is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return _timer.IsEnabled; }
        }

        /// <summary>
        /// Starts the wrapped timer.
        /// </summary>
        public void Start()
        {
            _timer.Start();
        }

        /// <summary>
        /// Stops the wrapped timer.
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Called when the wrapped timer ticks.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event-related data.</param>
        private void OnTimerTick(object sender, object e)
        {
            var handler = this.Tick;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
