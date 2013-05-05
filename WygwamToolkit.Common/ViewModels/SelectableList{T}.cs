//-----------------------------------------------------------------------
// <copyright file="SelectableList{T}.cs" company="Wygwam">
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: Provide summary section in the documentation header.
    /// </summary>
    public class SelectableList<T> : ObservableCollection<SelectableListItem<T>>
    {
        public event EventHandler<SelectableItemChangedEventArgs<T>> SelectedItemsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SelectableList"/> class.
        /// </summary>
        public SelectableList()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SelectableList"/> class.
        /// </summary>
        public SelectableList(IEnumerable<SelectableListItem<T>> collection)
            : base(collection)
        {
            foreach (var item in collection)
            {
                item.PropertyChanged += OnItemPropertyChanged;
            }
        }

        public IEnumerable<T> SelectedItems
        {
            get
            {
                return this.Where(si => si.IsSelected).Select(si => si.Item);
            }
        }

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems.Cast<SelectableListItem<T>>())
                {
                    item.PropertyChanged += OnItemPropertyChanged;
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in e.OldItems.Cast<SelectableListItem<T>>())
                {
                    item.PropertyChanged -= OnItemPropertyChanged;
                }
            }
        }

        protected void OnSelectedItemsChanged(SelectableItemChangedEventArgs<T> e)
        {
            var handler = this.SelectedItemsChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnSelectedItemsChanged(new SelectableItemChangedEventArgs<T>
                {
                    ChangedItem = sender as SelectableListItem<T>
                });
        }
    }
}
