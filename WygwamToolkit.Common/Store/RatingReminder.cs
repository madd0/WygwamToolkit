//-----------------------------------------------------------------------
// <copyright file="RatingReminder.cs" company="Wygwam">
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
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A base class that provides rating prompting functionality for apps.
    /// </summary>
    public abstract class RatingReminder
    {
        /// <summary>
        /// Increments the number of times the app has been run.
        /// </summary>
        /// <returns>A task that yields when the number of runs has been incremented and provides the new number as the result of the task.</returns>
        public abstract Task<int> IncrementRunCountAsync();

        /// <summary>
        /// Gets the number of times the app has been run.
        /// </summary>
        /// <returns>A task that yields when the number of runs has been retrieved and provides the number as the result of the task.</returns>
        public abstract Task<int> GetRunCountAsync();

        /// <summary>
        /// Gets a value that determines whether the app has already been rated.
        /// </summary>
        /// <returns>A task that yields when the value that determines whether the app has already been rated has been retrieved and provides it as the result of the task.</returns>
        public abstract Task<bool> GetIsRatedAsync();

        /// <summary>
        /// Displays a UI prompting the user to rate the app.
        /// </summary>
        /// <param name="settings">The settings that determine when the prompt must be displayed and the texts that it shows.</param>
        /// <returns>
        /// A task that yields if the prompt must not be shown or when the user dismisses it. The result
        /// contains a value that determines whether the prompt was shown and, if so, what the user answered.
        /// </returns>
        /// <remarks>
        /// The prompt should only be shown if the app has been ran the appropriate number of times
        /// and the user has not refused further prompts.
        /// </remarks>
        public Task<RatingReminderResult> TryShowReminderAsync(RatingReminderSettings settings)
        {
            return this.TryShowReminderAsync(settings, this.TryShowDefaultReminderAsync);
        }

        /// <summary>
        /// Displays a UI prompting the user to rate the app.
        /// </summary>
        /// <param name="settings">The settings that determine when the prompt must be displayed and the texts that it shows.</param>
        /// <param name="promptAction">The a delegate that will display a custom reminder. The delegate will receive as a parameter the
        /// <see cref="T:RatingReminderSettings" /> instanced passed as first parameter to the method in order to display the correct texts. It
        /// must return a task that yields when the user has made a choice.</param>
        /// <returns>
        /// A task that yields if the prompt must not be shown or when the user dismisses it. The result
        /// contains a value that determines whether the prompt was shown and, if so, what the user answered.
        /// </returns>
        /// <remarks>
        /// The prompt should only be shown if the app has been ran the appropriate number of times
        /// and the user has not refused further prompts.
        /// </remarks>
        public Task<RatingReminderResult> TryShowReminderAsync(RatingReminderSettings settings, Func<RatingReminderSettings, Task<RatingReminderResult>> promptAction)
        {
            return this.TryShowReminderAsync(settings, promptAction, this.DefaultMustPromptPredicate);
        }

        /// <summary>
        /// Displays a UI prompting the user to rate the app.
        /// </summary>
        /// <param name="settings">The settings that determine when the prompt must be displayed and the texts that it shows.</param>
        /// <param name="promptAction">The a delegate that will display a custom reminder. The delegate will receive as a parameter the
        /// <see cref="T:RatingReminderSettings" /> instanced passed as first parameter to the method in order to display the correct texts. It
        /// must return a task that yields when the user has made a choice.</param>
        /// <param name="mustPromptPredicate">An asynchronous predicate that returns <c>true</c> if the reminder prompt must be displayed; <c>false</c> otherwise.</param>
        /// <returns>
        /// A task that yields if the prompt must not be shown or when the user dismisses it. The result
        /// contains a value that determines whether the prompt was shown and, if so, what the user answered.
        /// </returns>
        /// <remarks>
        /// The prompt should only be shown if the app has been ran the appropriate number of times
        /// and the user has not refused further prompts.
        /// </remarks>
        public async Task<RatingReminderResult> TryShowReminderAsync(
            RatingReminderSettings settings, 
            Func<RatingReminderSettings,
            Task<RatingReminderResult>> promptAction, Func<RatingReminder, RatingReminderSettings, Task<bool>> mustPromptPredicate)
        {
            var result = RatingReminderResult.NotNeeded;

            if (await mustPromptPredicate(this, settings))
            {
                result = await promptAction(settings);

                switch (result)
                {
                    case RatingReminderResult.Rate:
                        await this.GoToRatingAsync();
                        break;
                    case RatingReminderResult.Later:
                        await this.ResetNumberOfRunsAsync();
                        break;
                    case RatingReminderResult.Never:
                        await this.StoreNeverRemindAsync();
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Displays the default reminder for a given platform.
        /// </summary>
        /// <param name="settings">The settings that determine when the prompt must be displayed and the texts that it shows.</param>
        /// <returns>
        /// A task that yields if the prompt must not be shown or when the user dismisses it. The result
        /// contains a value that determines whether the prompt was shown and, if so, what the user answered.
        /// </returns>
        protected abstract Task<RatingReminderResult> TryShowDefaultReminderAsync(RatingReminderSettings settings);

        /// <summary>
        /// Sends the user to the rating UI of the specific platform.
        /// </summary>
        /// <returns>A task that yields when the user has been sent to the rating UI.</returns>
        protected abstract Task GoToRatingAsync();

        /// <summary>
        /// Resets the counter that keeps track of the number times that the app has been launched.
        /// </summary>
        /// <returns>A task that yields when the counter has been reset.</returns>
        protected abstract Task ResetNumberOfRunsAsync();

        /// <summary>
        /// Stores a value to prevent the reminder from being shown again.
        /// </summary>
        /// <returns>A task that yields when the value has been stored.</returns>
        protected abstract Task StoreNeverRemindAsync();

        /// <summary>
        /// The default predicate that determines whether the reminder must be shown.
        /// </summary>
        /// <param name="ratingReminder">The instance of <see cref="T:RatingReminder"/> that is prompting the user to rate the app.</param>
        /// <param name="settings">The settings that determine when the prompt must be displayed and the texts that it shows.</param>
        /// <returns><c>true</c> if the reminder must be displayed; <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> DefaultMustPromptPredicate(RatingReminder ratingReminder, RatingReminderSettings settings)
        {
            return !(await ratingReminder.GetIsRatedAsync()) &&
                   (await this.GetRunCountAsync() >= settings.NumberOfRuns);
        }
    }
}
