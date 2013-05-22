//-----------------------------------------------------------------------
// <copyright file="ExtendedListView.cs" company="Wygwam">
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
    using System.Windows.Input;

    /// <summary>
    /// An extension of the WinRT <see cref="global::Windows.UI.Xaml.Controls.ListView"/>
    /// that provides a <see cref="P:Command"/> property.
    /// </summary>
    /// <remarks>The <see cref="System.Windows.Input.ICommand"/> bound to the <see cref="P:Command"/> 
    /// property will be executed when an item is clicked. The object associated with the item (its view model)
    /// will be passed to the command as a parameter.</remarks>
    public class ExtendedListView : ListView
    {
        /// <summary>
        /// Identifies the <see cref="P:Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(BindableSelectionListView), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command that will execute when an item of the list is clicked.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedListView"/> class.
        /// </summary>
        public ExtendedListView()
        {
            this.ItemClick += this.OnItemClick;
        }

        /// <summary>
        /// Called when an item on the list is clicked. This will execute the <see cref="P:Command"/> if
        /// any, passing as a command parameter, the data context of the clicked item.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The <see cref="ItemClickEventArgs"/> instance containing the event data.</param>
        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.Command != null && this.Command.CanExecute(e.ClickedItem))
            {
                this.Command.Execute(e.ClickedItem);
            }
        }
    }
}
