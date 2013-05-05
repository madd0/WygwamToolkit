//-----------------------------------------------------------------------
// <copyright file="SelectableItemChangedEventArgs.cs" company="Wygwam">
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

namespace Wygwam.Windows.ViewModels
{
    using System;

    /// <summary>
    /// Provides data for the <see cref="SelectableList{T}.SelectedItemsChanged"/> event.
    /// </summary>
    /// <typeparam name="T">The type of item stored in the <see cref="SelectableItem{T}"/> that was selected
    /// or deselected.</typeparam>
    public class SelectableItemChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemChangedEventArgs{T}" /> class.
        /// </summary>
        /// <param name="changedItem">The changed item.</param>
        public SelectableItemChangedEventArgs(SelectableItem<T> changedItem)
        {
            this.ChangedItem = changedItem;
        }

        /// <summary>
        /// Gets the changed item.
        /// </summary>
        public SelectableItem<T> ChangedItem
        {
            get;
            private set;
        }
    }
}
