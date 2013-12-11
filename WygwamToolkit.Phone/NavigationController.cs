//-----------------------------------------------------------------------
// <copyright file="NavigationController.cs" company="Wygwam">
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

namespace Wygwam.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Wygwam.Windows.ViewModels;

    /// <summary>
    /// Provides MVVM-compatible navigation for Windows Store Apps.
    /// </summary>
    public class NavigationController : BaseNavigationController
    {
        private readonly Dictionary<Type, Uri> _viewModelMap = new Dictionary<Type, Uri>();

        private Frame _currentNavigationFrame;

        private object _pendingNavigationParameter;

        private bool _isInternalGoBack;

        /// <summary>
        /// Associates a view type with a view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model. This must extend <see cref="BaseViewModel" />.</typeparam>
        /// <typeparam name="TView">The type of the view. This must be a <see cref="global::Windows.UI.Xaml.Controls.Page" />.</typeparam>
        /// <seealso cref="M:GoTo{TViewModel}" />
        /// <remarks>
        /// A new instance of the view model will be created at runtime by reflection when you
        /// navigate. The constructor of the view model will receive any parameters that you pass during navigation.
        /// </remarks>
        public void Register<TViewModel>(Uri pageUri)
            where TViewModel : BaseViewModel
        {
            this.Register<TViewModel>(pageUri, args => (TViewModel)Activator.CreateInstance(typeof(TViewModel), args));
        }

        /// <summary>
        /// Associates a view type with a view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model. This must extend <see cref="BaseViewModel" />.</typeparam>
        /// <typeparam name="TView">The type of the view. This must be a <see cref="global::Windows.UI.Xaml.Controls.Page" />.</typeparam>
        /// <param name="instance">An instance of the view model that will be used as a singleton during navigation operations.</param>
        /// <seealso cref="M:GoTo{TViewModel}" />
        /// <remarks>
        /// The same instance of the view model will used for every navigation operation.
        /// </remarks>
        public void Register<TViewModel>(Uri pageUri, TViewModel instance)
            where TViewModel : BaseViewModel
        {
            this.Register<TViewModel>(pageUri, _ => instance);
        }

        /// <summary>
        /// Associates a view type with a view model type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model. This must extend <see cref="BaseViewModel" />.</typeparam>
        /// <typeparam name="TView">The type of the view. This must be a <see cref="global::Windows.UI.Xaml.Controls.Page" />.</typeparam>
        /// <param name="delegate">A delegate that will create an instance of the view model during navigation.</param>
        /// <seealso cref="M:GoTo{TViewModel}" />
        /// <remarks>
        /// The provided delegate must return the desired instance of the view model. It will be provided an array of arguments
        /// that can be used to create or initialize the view model.
        /// </remarks>
        public void Register<TViewModel>(Uri pageUri, Func<object[], TViewModel> @delegate)
            where TViewModel : BaseViewModel
        {
            var type = typeof(TViewModel);

            _viewModelMap.Add(type, pageUri);

            this.RegisterViewModelGenerator<TViewModel>(type, @delegate);
        }

        public async void GoHome()
        {
            _currentNavigationFrame.Navigated -= this.OnFrameNavigated;

            var navigationService = ((Page)_currentNavigationFrame.Content).NavigationService;

            while (this.ViewModelNavigationStack.Count > 2)
            {
                navigationService.RemoveBackEntry();

                var disposable = this.ViewModelNavigationStack.Pop() as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            if (this.ViewModelNavigationStack.Count > 1)
            {
                await this.GoBackInternalAsync();

                var disposable = this.ViewModelNavigationStack.Pop() as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            _currentNavigationFrame.Navigated += this.OnFrameNavigated;

            var homeView = _currentNavigationFrame.Content as FrameworkElement;

            if (homeView != null)
            {
                homeView.DataContext = this.ViewModelNavigationStack.Peek();
            }

            await this.ViewModelNavigationStack.Peek().Reload();
        }

        protected override bool NavigateToType(Type viewModelType, object viewModelInstance)
        {
            var frame = this.WindowManager.EnsureNavigationFrameExists();

            if (frame != _currentNavigationFrame)
            {
                if (_currentNavigationFrame != null)
                {
                    _currentNavigationFrame.Navigated -= this.OnFrameNavigated;
                }

                _currentNavigationFrame = frame;
                _currentNavigationFrame.Navigated += this.OnFrameNavigated;
            }

            _pendingNavigationParameter = viewModelInstance;

            var navigationResult = frame.Navigate(_viewModelMap[viewModelType]);

            return navigationResult;
        }

        protected override bool IsViewModelRegistered(Type viewModelType)
        {
            return _viewModelMap.ContainsKey(viewModelType);
        }

        private void OnFrameNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var navigationTarget = e.Content as FrameworkElement;

            if (e.NavigationMode == NavigationMode.Back)
            {
                if (e.IsNavigationInitiator && !_isInternalGoBack)
                {
                    this.UnwrapNavigationStack(() => Task.FromResult(navigationTarget));
                }
            }
            else
            {
                // _pendingNavigationParameter is only null if the toolkit didn't initiate navigation
                if (_pendingNavigationParameter == null)
                {
                    // push dummy view model so that the back stack is unwrapped properly later
                    this.ViewModelNavigationStack.Push(new BaseViewModel());
                }
                else if (navigationTarget != null)
                {
                    navigationTarget.DataContext = _pendingNavigationParameter;
                    _pendingNavigationParameter = null;
                }
            }
        }

        protected override async Task<FrameworkElement> GoBackInternalAsync()
        {
            _isInternalGoBack = true;

            NavigatedEventHandler backNavigationHandler = null;

            TaskCompletionSource<FrameworkElement> taskSource = new TaskCompletionSource<FrameworkElement>();

            await SmartDispatcher.BeginInvoke(() =>
            {
                var frame = this.WindowManager.NavigationFrame;

                backNavigationHandler = (sender, args) =>
                {
                    taskSource.SetResult(args.Content as FrameworkElement);

                    _isInternalGoBack = false;

                    frame.Navigated -= backNavigationHandler;
                };

                frame.Navigated += backNavigationHandler;

                frame.GoBack();
            });

            return await taskSource.Task;
        }
    }
}
