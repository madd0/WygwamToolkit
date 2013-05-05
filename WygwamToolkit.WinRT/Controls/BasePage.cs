//-----------------------------------------------------------------------
// <copyright file="BasePage.cs" company="Wygwam">
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
    using global::Windows.UI.Xaml.Navigation;
    using Wygwam.Windows.ViewModels;

    /// <summary>
    /// TODO: Provide summary section in the documentation header.
    /// </summary>
    public class BasePage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasePage"/> class.
        /// </summary>
        public BasePage()
        {
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as BaseViewModel;

            if (vm != null)
            {
                vm.OnLoaded();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = e.Parameter;
        }
    }
}
