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
    using global::Windows.ApplicationModel.Activation;
    using global::Windows.Foundation;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;
    using global::Windows.UI.Xaml.Media.Imaging;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Wygwam.Windows.ViewModels;

    /// <summary>
    /// Provides a basic extended splash screen that follows Microsoft guidelines.
    /// </summary>
    public class ExtendedSplashScreen : BaseExtendedSplashScreen
    {
        private NavigationController _navigationController;

        private SplashScreen _splashScreen;

        private Image _splashScreenImage;
        private Uri _splashScreenImagePath;

        private Panel _panel;

        private Brush _backgroundBrush;

        private Type _nextPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedSplashScreen"/> class.
        /// </summary>
        public ExtendedSplashScreen()
            : base()
        {
            this.DefaultStyleKey = typeof(ExtendedSplashScreen);

            Window.Current.SizeChanged += this.OnResize;

            this.LoadManifestData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedSplashScreen"/> class.
        /// </summary>
        /// <param name="navigationController">The navigation controller used to navigate away from the splash screen.</param>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance provided by the
        /// application when it was launched.</param>
        /// <param name="nextPage">The type of the page to which the application will navigate when loading is done.</param>
        /// <param name="parameter">An optional navigation parameter.</param>
        public ExtendedSplashScreen(NavigationController navigationController, IActivatedEventArgs args, Type nextPage, object parameter = null)
            : this()
        {
            _navigationController = navigationController;

            _nextPage = nextPage;

            this.NavigationParameter = parameter;

            this.LaunchArgs = args;
        }

        /// <summary>
        /// Gets or sets an object containing details about the launch request and process.
        /// </summary>
        public override IActivatedEventArgs LaunchArgs
        {
            get
            {
                return base.LaunchArgs;
            }
            set
            {
                base.LaunchArgs = value;

                _splashScreen = value.SplashScreen;

                if (_splashScreen != null)
                {
                    _splashScreen.Dismissed += this.OnSplashScreenDismissed;
                }
            }
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        /// call ApplyTemplate. In simplest terms, this means the method is called just before a UI element 
        /// displays in your app. Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _splashScreenImage = this.GetTemplateChild("SplashScreenImage") as Image;

            if (_splashScreenImage != null)
            {
                _splashScreenImage.ImageOpened += this.OnImageOpened;
                _splashScreenImage.Source = new BitmapImage(_splashScreenImagePath);
            }

            _panel = this.GetTemplateChild("RootPanel") as Panel;

            if (_panel != null)
            {
                _panel.Background = _backgroundBrush;
            }

            this.PositionImage();
        }

        /// <summary>
        /// Called when the screen changes size and the splash screen images is repositioned.
        /// </summary>
        /// <param name="splashScreenPosition">The splash screen position.</param>
        protected virtual void OnResize(Rect splashScreenPosition)
        {
        }

        protected override void MoveToNextPage()
        {
            if (_baseViewModelTypeInfo.IsAssignableFrom(_nextPage.GetTypeInfo()))
            {
                _navigationController.GoToInstance((BaseViewModel)this.NavigationParameter);
            }
            else
            {
                var rootFrame = new Frame();

                rootFrame.Navigate(_nextPage, this.NavigationParameter);

                Window.Current.Content = rootFrame;
            }
        }

        /// <summary>
        /// Called when the splash screen image has been opened. The system splash screen will be dismissed
        /// a few milliseconds afterwards to avoid flickering.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void OnImageOpened(object sender, RoutedEventArgs e)
        {
            await Task.Delay(50);

            Window.Current.Activate();
        }

        /// <summary>
        /// Called when the extended splash screen is resized. This is important to ensure that the extended splash 
        /// screen is formatted properly in response to snapping, unsnapping, rotation, etc...
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="WindowSizeChangedEventArgs"/> instance containing the event data.</param>
        private void OnResize(object sender, WindowSizeChangedEventArgs e)
        {
            this.PositionImage();
        }

        /// <summary>
        /// Called when the system splash screen is dismissed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnSplashScreenDismissed(SplashScreen sender, object e)
        {
            await this.PerformLoadingActionAsync();

            await SmartDispatcher.BeginInvoke(() =>
            {
                if (this.AdvancesAutomatically)
                {
                    this.IsDone = true;
                }
            });
        }

        /// <summary>
        /// Loads the path to the splash screen and the background color from the application manifest.
        /// </summary>
        private void LoadManifestData()
        {
            var doc = XDocument.Load("AppxManifest.xml", LoadOptions.None);
            var xname = XNamespace.Get("http://schemas.microsoft.com/appx/2010/manifest");

            var splashScreenElement = doc.Descendants(xname + "SplashScreen").First();

            var splashScreenPath = splashScreenElement.Attribute("Image").Value;

            _splashScreenImagePath = new Uri(new Uri("ms-appx:///"), splashScreenPath);

            XAttribute bgColor = null;

            if ((bgColor = splashScreenElement.Attribute("BackgroundColor")) == null)
            {
                bgColor = doc.Descendants(xname + "VisualElements").First().Attribute("BackgroundColor");
            }

            _backgroundBrush = new SolidColorBrush(bgColor.Value.AsColor());
        }

        /// <summary>
        /// Positions the extended splash screen image in the same location as the system splash screen image.
        /// </summary>
        private void PositionImage()
        {
            if (_splashScreenImage != null && _splashScreen != null)
            {
                var imgPos = _splashScreen.ImageLocation;

                _splashScreenImage.SetValue(Canvas.LeftProperty, imgPos.X);
                _splashScreenImage.SetValue(Canvas.TopProperty, imgPos.Y);
                _splashScreenImage.Height = imgPos.Height;
                _splashScreenImage.Width = imgPos.Width;

                this.OnResize(imgPos);
            }
        }
    }
}