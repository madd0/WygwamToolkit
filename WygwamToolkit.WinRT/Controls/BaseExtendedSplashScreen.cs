//-----------------------------------------------------------------------
// <copyright file="BaseExtendedSplashScreen.cs" company="Wygwam">
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
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Xml.Linq;
    using Wygwam.Windows.ViewModels;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
#elif WINDOWS_PHONE
    using System.Windows;
    using Microsoft.Phone.Controls;
#endif

    /// <summary>
    /// Provides a basic extended splash screen that follows Microsoft guidelines.
    /// </summary>
#if WINDOWS_PHONE
    public abstract class BaseExtendedSplashScreen : PhoneApplicationPage
#else
    public abstract class BaseExtendedSplashScreen : Control
#endif
    {
        /// <summary>
        /// Identifies the <see cref="P:Message"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(BaseExtendedSplashScreen), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="P:IsLoading"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(BaseExtendedSplashScreen), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="P:Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(BaseExtendedSplashScreen), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="P:CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(BaseExtendedSplashScreen), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="P:NavigationParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationParameterProperty =
            DependencyProperty.Register("NavigationParameter", typeof(object), typeof(BaseExtendedSplashScreen), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="P:AdvancesAutomatically"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AdvancesAutomaticallyProperty =
            DependencyProperty.Register("AdvancesAutomatically", typeof(bool), typeof(BaseExtendedSplashScreen), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="P:IsDone"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDoneProperty =
            DependencyProperty.Register("IsDone", typeof(bool), typeof(BaseExtendedSplashScreen), new PropertyMetadata(false, OnIsDoneChanged));

        protected static readonly TypeInfo _baseViewModelTypeInfo = typeof(BaseViewModel).GetTypeInfo();

        private TaskScheduler _uiScheduler;

        private IActivatedEventArgs _launchArgs;

        private bool _mustLoadPreviousRunningState;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseExtendedSplashScreen"/> class.
        /// </summary>
        public BaseExtendedSplashScreen()
        {
            this.UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        /// <summary>
        /// Gets the scheduler that can be used to execute tasks on the UI thread.
        /// </summary>
        public TaskScheduler UIScheduler
        {
            get { return _uiScheduler; }
            private set { _uiScheduler = value; }
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
        public virtual IActivatedEventArgs LaunchArgs
        {
            get
            {
                return _launchArgs;
            }

            set
            {
                _launchArgs = value;

                _mustLoadPreviousRunningState = _launchArgs.PreviousExecutionState == ApplicationExecutionState.Terminated;
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

                var disposableViewModel = this.DataContext as IDisposable;

                if (disposableViewModel != null)
                {
                    disposableViewModel.Dispose();
                }
            }
        }

        /// <summary>
        /// Moves to next page.
        /// </summary>
        protected abstract void MoveToNextPage();

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
        protected async Task PerformLoadingActionAsync()
        {
            TaskFactory uiFactory = new TaskFactory(this.UIScheduler);

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
