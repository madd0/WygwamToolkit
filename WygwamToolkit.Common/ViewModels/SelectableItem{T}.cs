//-----------------------------------------------------------------------
// <copyright file="SelectableItem{T}.cs" company="Wygwam">
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
    /// <summary>
    /// Used as a view model to provide items to lists.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public class SelectableItem<T> : BindableBase
    {
        private bool _isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItem{T}" /> class.
        /// </summary>
        /// <param name="item">The item to be stored.</param>
        /// <param name="isSelected">if set to <c>true</c>, the item will be selected by default.</param>
        public SelectableItem(T item, bool isSelected = false)
        {
            this.Item = item;
            this.IsSelected = isSelected;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                this.SetProperty(ref _isSelected, value);
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public T Item { get; private set; }
    }
}
