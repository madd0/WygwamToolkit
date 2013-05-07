//-----------------------------------------------------------------------
// <copyright file="ITimer.cs" company="Wygwam">
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
