//-----------------------------------------------------------------------
// <copyright file="BindableSelectionGridView.cs" company="Wygwam">
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
namespace Wygwam.Windows.Controls
{
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Controls.Primitives;
    using global::Windows.UI.Xaml.Data;

    /// <summary>
    /// An extension of the WinRT <see cref="global::Windows.UI.Xaml.Controls.GridView"/>
    /// that binds the <see cref="P:SelectorItem.IsSelected"/> property of its
    /// <see cref="GridViewItem"/> elements to the <c>IsSelected</c> property of the items
    /// in its <see cref="P:ItemsControl.ItemsSource"/> collection.
    /// </summary>
    /// <remarks>Due to the dynamic nature of binding, any type of object with an <c>IsSelected</c>
    /// property will work, but the toolkit provides <see cref="T:Wygwam.Windows.ViewModels.SelectableItem<T>"/>
    /// for this purpose.</remarks>
    public class BindableSelectionGridView : GridView
    {
        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element that's used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            Binding binding = new Binding()
            {
                Source = item,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("IsSelected")
            };

            ((SelectorItem)element).SetBinding(SelectorItem.IsSelectedProperty, binding);
        }
    }
}
