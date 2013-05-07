//-----------------------------------------------------------------------
// <copyright file="Timer.cs" company="Wygwam">
//   MS-PL
// </copyright>
// <license>
//   This source code is subject to terms and conditions of the Microsoft 
//   Public License. A copy of the license can be found in the LICENSE.md 
//   file at the root of this distribution. 
//   By using this source code in any fashion, you are agreeing to be bound 
//   by the terms of the Microsoft Public License. You must not remove this 
//   notice, or any other, from this software.
// </license>
//-----------------------------------------------------------------------

namespace Wygwam.Windows.WinRT
{
    using global::Windows.UI.Xaml;
    using System;

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
