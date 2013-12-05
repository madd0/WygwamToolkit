﻿//-----------------------------------------------------------------------
// <copyright file="RatingReminderSettings.cs" company="Wygwam">
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

namespace Wygwam.Windows.Store
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides settings to an instance of <see cref="T:IRatingReminder"/>.
    /// </summary>
    public class RatingReminderSettings
    {
        private const int DEFAULT_NUMBER_OF_RUNS = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingReminderSettings"/> class.
        /// </summary>
        public RatingReminderSettings()
            : this(DEFAULT_NUMBER_OF_RUNS)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingReminderSettings"/> class.
        /// </summary>
        /// <param name="numberOfRuns">The number of runs before the user is prompted to rate the app.</param>
        public RatingReminderSettings(int numberOfRuns)
            : this(numberOfRuns, new List<RatingReminderAction>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingReminderSettings"/> class.
        /// </summary>
        /// <param name="actions">The actions.</param>
        public RatingReminderSettings(IList<RatingReminderAction> actions)
            : this(DEFAULT_NUMBER_OF_RUNS, actions)
        {
        }

        public RatingReminderSettings(int numberOfRuns, IEnumerable<RatingReminderAction> actions)
        {
            this.NumberOfRuns = numberOfRuns;
            this.Actions = new List<RatingReminderAction>();

            this.Actions.AddRange(actions);
        }

        /// <summary>
        /// Gets or sets the number of times that the application must be run before the reminder appears.
        /// </summary>
        public int NumberOfRuns { get; set; }

        /// <summary>
        /// Gets or sets the title of the reminder view.
        /// </summary>
        public string ReminderTitle { get; set; }

        /// <summary>
        /// Gets or sets the text of the reminder view.
        /// </summary>
        public string ReminderText { get; set; }

        /// <summary>
        /// Gets the list of actions to be displayed in the prompt.
        /// </summary>
        public IList<RatingReminderAction> Actions { get; private set; }
    }
}
