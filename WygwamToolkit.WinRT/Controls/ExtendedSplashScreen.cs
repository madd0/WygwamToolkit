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
    using System.Windows.Input;
    using System.Xml.Linq;
    using Wygwam.Windows.ViewModels;

    /// <summary>
    /// Provides a basic extended splash screen that follows Microsoft guidelines.
    /// </summary>
    public class ExtendedSplashScreen : Control
    {
        /// <summary>
        /// Identifies the <see cref="P:Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ExtendedSplashScreen), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="P:IsLoading"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(ExtendedSplashScreen), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="P:Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ExtendedSplashScreen), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="P:CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ExtendedSplashScreen), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="P:NavigationParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationParameterProperty =
            DependencyProperty.Register("NavigationParameter", typeof(object), typeof(ExtendedSplashScreen), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="P:AdvancesAutomatically"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AdvancesAutomaticallyProperty =
            DependencyProperty.Register("AdvancesAutomatically", typeof(bool), typeof(ExtendedSplashScreen), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="P:IsDone"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDoneProperty =
            DependencyProperty.Register("IsDone", typeof(bool), typeof(ExtendedSplashScreen), new PropertyMetadata(false, OnIsDoneChanged));

        private static readonly TypeInfo _baseViewModelTypeInfo = typeof(BaseViewModel).GetTypeInfo();

        private SplashScreen _splashScreen;

        private Image _splashScreenImage;
        private Uri _splashScreenImagePath;

        private Panel _panel;

        private Brush _backgroundBrush;

        private Type _nextPage;

        private TaskScheduler _uiScheduler;

        private LaunchActivatedEventArgs _launchArgs;

        private bool _mustLoadPreviousRunningState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedSplashScreen"/> class.
        /// </summary>
        public ExtendedSplashScreen()
        {
            this.DefaultStyleKey = typeof(ExtendedSplashScreen);

            Window.Current.SizeChanged += this.OnResize;

            _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            this.LoadManifestData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedSplashScreen"/> class.
        /// </summary>
        /// <param name="args">The <see cref="LaunchActivatedEventArgs"/> instance provided by the
        /// <see cref="global::Windows.UI.Xaml.Application"/> when it was launched.</param>
        /// <param name="nextPage">The type of the page to which the application will navigate when loading is done.</param>
        /// <param name="parameter">An optional navigation parameter.</param>
        public ExtendedSplashScreen(LaunchActivatedEventArgs args, Type nextPage, object parameter = null)
            : this()
        {
            _nextPage = nextPage;

            this.NavigationParameter = parameter;

            this.LaunchArgs = args;
        }

        /// <summary>
        /// Gets or sets the loading operation.
        /// </summary>
        /// <value>
        /// The loading operation.
        /// </value>
        /// <seealso cref="P:Command" />
        /// <remarks>
        ///   <para>This delegate will be called when the system splash screen has been dismissed
        /// in order to perform loading operations.</para>
        ///   <para>You can also use the class's <see cref="P:Command" /> property to execute loading operations.</para>
        /// </remarks>
        public Func<bool, Task> LoadingOperation { get; set; }

        /// <summary>
        /// Gets or sets an object containing details about the launch request and process.
        /// </summary>
        public LaunchActivatedEventArgs LaunchArgs
        {
            get
            {
                return _launchArgs;
            }

            set
            {
                _launchArgs = value;

                _mustLoadPreviousRunningState = _launchArgs.PreviousExecutionState == ApplicationExecutionState.Terminated;

                _splashScreen = _launchArgs.SplashScreen;

                if (_splashScreen != null)
                {
                    _splashScreen.Dismissed += this.OnSplashScreenDismissed;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the splash screen advances automatically to the next
        /// page once the <see cref="P:Command"/> or <see cref="P:LoadingOperation"/> have completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the splash screen is automatically dismissed after the loading operations have
        /// completed; otherwise, <c>false</c>. The defaul value is <c>true</c>.
        /// </value>
        /// <remarks>Although, by default, the extended splash screen will advance to the next page that 
        /// was specified in its constructor, if you require finer control, you can set this property to <c>false</c>
        /// and control when navigation occurs using the <see cref="P:IsDone"/> property.</remarks>
        public bool AdvancesAutomatically
        {
            get { return (bool)GetValue(AdvancesAutomaticallyProperty); }
            set { SetValue(AdvancesAutomaticallyProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether loading is done.
        /// </summary>
        /// <remarks>If <see cref="P:AdvancesAutomatically"/> has been set to <c>false</c>, setting
        /// this property to <c>true</c> will trigger navigation to the next page.</remarks>
        public bool IsDone
        {
            get { return (bool)GetValue(IsDoneProperty); }
            set { SetValue(IsDoneProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command that will execute the loading operations when the system splash screen
        /// is dismissed.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets a parameter that will be passed on to the <see cref="P:Command"/> when it is executed.
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the splash screen message.
        /// </summary>
        /// <remarks>The default template for the extended splash screen contains a 
        /// <see cref="global::Windows.UI.Xaml.Controls.TextBlock"/> that displays this message.</remarks>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the splash screen is waiting.
        /// </summary>
        /// <remarks>The default template for the extended splash screen contains a 
        /// <see cref="global::Windows.UI.Xaml.Controls.ProgressRing"/> that is activated while this property
        /// is <c>true</c>.</remarks>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that will used as a parameter when the extended splash screen navigates to the next page.
        /// </summary>
        public object NavigationParameter
        {
            get { return (object)GetValue(NavigationParameterProperty); }
            set { SetValue(NavigationParameterProperty, value); }
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
            else
            {
                this.Loaded += this.OnLoaded;
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

        /// <summary>
        /// Called when the <see cref="P:IsDone"/> dependency property changes.
        /// </summary>
        /// <param name="d">The object that triggered the event.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsDoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExtendedSplashScreen)d).OnIsDoneChanged((bool)e.NewValue);
        }

        /// <summary>
        /// Called when the <see cref="P:IsDone"/> dependency property changes.
        /// </summary>
        /// <param name="isDone">if set to <c>true</c>, loading has completed and the application should advance to the next page.</param>
        private void OnIsDoneChanged(bool isDone)
        {
            if (isDone)
            {
                this.MoveToNextPage();
            }
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
        /// Called when the control is loaded. This method is only called if no splash screen image was detected
        /// in order to bring the control into view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnLoaded;

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
            await this.PerformLoadingActionAsync().ContinueWith(
                _ =>
                {
                    if (this.AdvancesAutomatically)
                    {
                        this.IsDone = true;
                    }
                },
                _uiScheduler);
        }

        /// <summary>
        /// Moves to next page.
        /// </summary>
        private void MoveToNextPage()
        {
            if (_baseViewModelTypeInfo.IsAssignableFrom(_nextPage.GetTypeInfo()))
            {
                NavigationController.GoToInstance((BaseViewModel)this.NavigationParameter);
            }
            else
            {
                var rootFrame = new Frame();

                rootFrame.Navigate(_nextPage, this.NavigationParameter);

                Window.Current.Content = rootFrame;
            }

            var disposableDataContext = this.DataContext as IDisposable;

            if (disposableDataContext != null)
            {
                disposableDataContext.Dispose();
            }
        }

        /// <summary>
        /// Performs the loading action asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task" /> that will complete when both event handlers and the
        /// <see cref="P:Command" /> have finished executing.
        /// </returns>
        /// <remarks>
        ///   <para>This method will call any event handler listening to the <see cref="P:LoadingOperation" /> event
        /// as well as execute the <see cref="P:Command" /> if one has been assigned.</para>
        ///   <para>Both operations are performed on the UI thread to make UI updates less complex.</para>
        /// </remarks>
        private async Task PerformLoadingActionAsync()
        {
            TaskFactory uiFactory = new TaskFactory(_uiScheduler);

            Task<Task> evenHandlerTask = uiFactory.StartNew(() =>
            {
                if (this.LoadingOperation != null)
                {
                    return this.LoadingOperation(_mustLoadPreviousRunningState);
                }
                else
                {
                    return Task.FromResult(false);
                }
            });

            Task<Task> commandTask = uiFactory.StartNew(() => this.ExecuteCommand());

            await Task.WhenAll(
                await evenHandlerTask,
                await commandTask);
        }

        /// <summary>
        /// Executes the <see cref="P:Command" /> if any.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task" /> to simplify asynchronous programming.
        /// </returns>
        /// <remarks>
        /// The method will call <see cref="M:ICommand.Execute" /> to execute the command, unless
        /// it detects that the command provided is of type <see cref="AsyncDelegateCommand" />, in which
        /// case it will await method <see cref="M:AsyncDelegateCommand.ExecuteAsync" />.
        /// </remarks>
        private async Task ExecuteCommand()
        {
            IAsyncExecutable asyncCommand;

            if (this.Command != null)
            {
                if ((asyncCommand = this.Command as IAsyncExecutable) != null)
                {
                    await asyncCommand.ExecuteAsync(this.CommandParameter);
                }
                else
                {
                    this.Command.Execute(this.CommandParameter);
                }
            }
        }
    }
}
