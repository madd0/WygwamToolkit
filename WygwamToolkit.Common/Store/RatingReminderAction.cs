//-----------------------------------------------------------------------
// <copyright file="RatingReminderAction.cs" company="Wygwam">
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
    /// <summary>
    /// Describes an actionable element in a rating reminder prompt.
    /// </summary>
    public class RatingReminderAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RatingReminderAction"/> class.
        /// </summary>
        /// <param name="label">The label of the actionable element.</param>
        /// <param name="result">The result returned when the element is selected by the user.</param>
        public RatingReminderAction(string label, RatingReminderResult result)
        {
            this.Label = label;

            this.Result = result;
        }

        /// <summary>
        /// Gets the text of the actionable element.
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the value that must be returned when the element is selected by the user.
        /// </summary>
        public RatingReminderResult Result { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the action that is executed when the user cancels.
        /// </summary>
        public bool IsCancelAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the action that is executed when the user cancels.
        /// </summary>
        public bool IsDefaultAction { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="T:RatingReminderAction" /> with the provided label
        /// whose action is <see cref="RatingReminderResult.Rate" />.
        /// </summary>
        /// <param name="label">The label of the actionable element.</param>
        /// <returns>
        /// An instance of <see cref="T:RatingReminderAction" /> with the provided label
        /// whose action is <see cref="RatingReminderResult.Rate" />.
        /// </returns>
        public static RatingReminderAction Rate(string label)
        {
            return new RatingReminderAction(label, RatingReminderResult.Rate);
        }

        /// <summary>
        /// Creates an instance of <see cref="T:RatingReminderAction" /> with the provided label
        /// whose action is <see cref="RatingReminderResult.Rate" />.
        /// </summary>
        /// <param name="label">The label of the actionable element.</param>
        /// <returns>
        /// An instance of <see cref="T:RatingReminderAction" /> with the provided label
        /// whose action is <see cref="RatingReminderResult.Later" />.
        /// </returns>
        public static RatingReminderAction Later(string label)
        {
            return new RatingReminderAction(label, RatingReminderResult.Later);
        }

        /// <summary>
        /// Creates an instance of <see cref="T:RatingReminderAction" /> with the provided label
        /// whose action is <see cref="RatingReminderResult.Rate" />.
        /// </summary>
        /// <param name="label">The label of the actionable element.</param>
        /// <returns>
        /// An instance of <see cref="T:RatingReminderAction" /> with the provided label
        /// whose action is <see cref="RatingReminderResult.Never" />.
        /// </returns>
        public static RatingReminderAction Never(string label)
        {
            return new RatingReminderAction(label, RatingReminderResult.Never);
        }
    }
}
