//-----------------------------------------------------------------------
// <copyright file="ExtendedSplashScreen.cs" company="Wygwam">
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
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Wygwam.Windows.ViewModels;
    using System.Reflection;

    public partial class ExtendedSplashScreen : BaseExtendedSplashScreen
    {
        private NavigationController _navigationController;

        private Type _nextPage;

        public ExtendedSplashScreen()
        {
            this.FontFamily = (FontFamily)Application.Current.Resources["PhoneFontFamilyNormal"];
                          this.FontSize=(double)Application.Current.Resources["PhoneFontSizeNormal"];
                          this.Foreground=(Brush)Application.Current.Resources["PhoneForegroundBrush"];
            
            this.Orientation = PageOrientation.Portrait;
            this.SupportedOrientations = SupportedPageOrientation.Portrait;

            this.SetValue(SystemTray.IsVisibleProperty, false);

            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedSplashScreen"/> class.
        /// </summary>
        /// <param name="navigationController">The navigation controller used to navigate away from the splash screen.</param>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance provided by the
        /// application when it was launched.</param>
        /// <param name="nextPage">The type of the page to which the application will navigate when loading is done.</param>
        /// <param name="parameter">An optional navigation parameter.</param>
        public ExtendedSplashScreen(NavigationController navigationController, Type nextPageViewModel, object parameter = null)
            : this()
        {
            _navigationController = navigationController;

            _nextPage = nextPageViewModel;

            this.NavigationParameter = parameter;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await this.PerformLoadingActionAsync().ContinueWith(
                _ =>
                {
                    if (this.AdvancesAutomatically)
                    {
                        this.IsDone = true;
                    }
                },
                this.UIScheduler);
        }

        protected override void MoveToNextPage()
        {
            if (_baseViewModelTypeInfo.IsAssignableFrom(_nextPage.GetTypeInfo()))
            {
                _navigationController.GoToInstance((BaseViewModel)this.NavigationParameter);
            }
            else
            {
                _navigationController.GoToType(_nextPage);
            }

            var nextPage = _navigationController.CurrentView;

            if (nextPage != null)
            {
                nextPage.NavigationService.RemoveBackEntry();
            }
        }
    }
}