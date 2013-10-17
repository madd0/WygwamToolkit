//-----------------------------------------------------------------------
// <copyright file="RatingReminderResult.cs" company="Wygwam">
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
    /// The possible values returned by a rating reminder prompt.
    /// </summary>
    public enum RatingReminderResult
    {
        /// <summary>
        /// The user should be forwarded to the app's rating page.
        /// </summary>
        Rate,

        /// <summary>
        /// The user should be prompted again at a later date.
        /// </summary>
        Later,

        /// <summary>
        /// The user should not be prompted again to rate the app.
        /// </summary>
        Never,

        /// <summary>
        /// The prompt was not needed.
        /// </summary>
        NotNeeded
    }
}
