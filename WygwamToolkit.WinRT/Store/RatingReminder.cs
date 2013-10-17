//-----------------------------------------------------------------------
// <copyright file="IRatingReminder.cs" company="Wygwam">
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

namespace Wygwam.Windows.WinRT.Store
{
    using global::Windows.ApplicationModel;
    using global::Windows.System;
    using global::Windows.UI.Popups;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Wygwam.Windows.Store;

    /// <summary>
    /// A base class that provides rating prompting functionality for apps.
    /// </summary>
    public class RatingReminder : Wygwam.Windows.Store.RatingReminder
    {
        private static readonly string RunCounterKey = "@RATE_RUN_COUNTER";

        private static readonly string IsRatedKey = "@RATE_IS_RATED";

        private static RatingReminder _instance;

        private int _cachedRunCount = -1;

        private bool? _cachedIsRated = null;

        /// <summary>
        /// Prevents a default instance of the <see cref="RatingReminder"/> class from being created.
        /// </summary>
        private RatingReminder()
        {
        }

        /// <summary>
        /// Gets a singleton instance of <see cref="T:RatingReminder"/>.
        /// </summary>
        public static RatingReminder Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RatingReminder();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Increments the number of times the app has been run.
        /// </summary>
        /// <returns>
        /// A task that yields when the number of runs has been incremented and provides the new number as the result of the task.
        /// </returns>
        public override async Task<int> IncrementRunCountAsync()
        {
            WinRT.StorageManager manager = new WinRT.StorageManager();

            var currentValue = await manager.LoadSettingAsync<int>(RunCounterKey, Storage.StorageType.Roaming);
            
            currentValue++;

            await manager.SaveSettingAsync(RunCounterKey, currentValue, Storage.StorageType.Roaming);

            return (_cachedRunCount = currentValue);
        }

        /// <summary>
        /// Gets the number of times the app has been run.
        /// </summary>
        /// <returns>
        /// A task that yields when the number of runs has been retrieved and provides the number as the result of the task.
        /// </returns>
        public override async Task<int> GetRunCountAsync()
        {
            if (_cachedRunCount == -1)
            {
                WinRT.StorageManager manager = new WinRT.StorageManager();

                _cachedRunCount = await manager.LoadSettingAsync<int>(RunCounterKey, Storage.StorageType.Roaming);
            }

            return _cachedRunCount;
        }

        /// <summary>
        /// Gets a value that determines whether the app has already been rated.
        /// </summary>
        /// <returns>
        /// A task that yields when the value that determines whether the app has already been rated has been retrieved and provides it as the result of the task.
        /// </returns>
        public override async Task<bool> GetIsRatedAsync()
        {
            if (!_cachedIsRated.HasValue)
            {
                WinRT.StorageManager manager = new WinRT.StorageManager();

                _cachedIsRated = await manager.LoadSettingAsync<bool>(IsRatedKey, Storage.StorageType.Roaming);
            }

            return _cachedIsRated.Value;
        }

        /// <summary>
        /// Displays the default reminder for a given platform.
        /// </summary>
        /// <param name="settings">The settings that determine when the prompt must be displayed and the texts that it shows.</param>
        /// <returns>
        /// A task that yields if the prompt must not be shown or when the user dismisses it. The result
        /// contains a value that determines whether the prompt was shown and, if so, what the user answered.
        /// </returns>
        protected override async Task<RatingReminderResult> TryShowDefaultReminderAsync(RatingReminderSettings settings)
        {
            var dialog = new MessageDialog(settings.ReminderText, settings.ReminderTitle);

            dialog.Commands.AddRange(settings.Actions.Select(action => new UICommand(action.Label, _ => { }, action)));

            for (int i = 0; i < settings.Actions.Count; i++)
            {
                if (settings.Actions[i].IsDefaultAction)
                {
                    dialog.DefaultCommandIndex = (uint)i;
                }
                else if (settings.Actions[i].IsCancelAction)
                {
                    dialog.CancelCommandIndex = (uint)i;
                }
            }

            var result = await dialog.ShowAsync();

            return result.Id != null ? ((RatingReminderAction)result.Id).Result : RatingReminderResult.NotNeeded;
        }

        /// <summary>
        /// Sends the user to the rating UI of the specific platform.
        /// </summary>
        /// <returns>
        /// A task that yields when the user has been sent to the rating UI.
        /// </returns>
        protected override async Task GoToRatingAsync()
        {
            var uri = string.Format(CultureInfo.InvariantCulture, "ms-windows-store:REVIEW?PFN={0}", Package.Current.Id.FamilyName);

            WinRT.StorageManager manager = new WinRT.StorageManager();

            await this.SetIsRated();

            await Launcher.LaunchUriAsync(new Uri(uri));
        }

        /// <summary>
        /// Resets the counter that keeps track of the number times that the app has been launched.
        /// </summary>
        /// <returns>
        /// A task that yields when the counter has been reset.
        /// </returns>
        protected override async Task ResetNumberOfRunsAsync()
        {
            WinRT.StorageManager manager = new WinRT.StorageManager();

            await manager.SaveSettingAsync(RunCounterKey, 0, Storage.StorageType.Roaming);

            _cachedRunCount = 0;
        }

        /// <summary>
        /// Stores a value to prevent the reminder from being shown again.
        /// </summary>
        /// <returns>
        /// A task that yields when the value has been stored.
        /// </returns>
        protected override Task StoreNeverRemindAsync()
        {
            return this.SetIsRated();
        }

        private async Task SetIsRated()
        {
            WinRT.StorageManager manager = new WinRT.StorageManager();

            await manager.SaveSettingAsync(IsRatedKey, true, Storage.StorageType.Roaming);

            _cachedIsRated = true;
        }
    }
}
