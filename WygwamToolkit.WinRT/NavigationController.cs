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
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using System;
    using System.Collections.Generic;
    using Wygwam.Windows.ViewModels;

    /// <summary>
    /// Provides MVVM-compatible navigation for Windows Store Apps.
    /// </summary>
    public static class NavigationController
    {
        private static readonly Dictionary<Type, Type> _viewModelMap = new Dictionary<Type, Type>();

        private static readonly Dictionary<Type, Func<object[], BaseViewModel>> _viewModelGenerators = new Dictionary<Type, Func<object[], BaseViewModel>>();

        private static readonly Stack<BaseViewModel> _viewModels = new Stack<BaseViewModel>();

        /// <summary>
        /// Provides access to the current <see cref="global::Windows.UI.Xaml.Controls.Frame"/>, if
        /// one exists.
        /// </summary>
        public static Frame CurrentFrame
        {
            get
            {
                return Window.Current.Content as Frame;
            }
            private set
            {
                Window.Current.Content = value;
            }
        }

        /// <summary>
        /// Gets the current view model.
        /// </summary>
        public static BaseViewModel CurrentViewModel
        {
            get
            {
                return _viewModels.Peek() as BaseViewModel;
            }
        }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        public static Page CurrentView
        {
            get
            {
                return CurrentFrame != null ? CurrentFrame.Content as Page : null;
            }
        }

        /// <summary>
        /// Goes back in the navigation stack.
        /// </summary>
        public static void GoBack()
        {
            if (CurrentFrame != null && CurrentFrame.CanGoBack)
            {
                BaseViewModel viewModel = _viewModels.Pop();

                CurrentFrame.GoBack();

                var currentPage = CurrentFrame.Content as Page;

                if (currentPage != null)
                {
                    if (_viewModels.Count > 0)
                    {
                        currentPage.DataContext = _viewModels.Peek();
                    }

                    BaseViewModel currentViewModel = currentPage.DataContext as BaseViewModel;

                    // TODO: Make sure this is necessary
                    if (currentViewModel != null)
                    {
                        currentViewModel.Reload();
                    }
                }
            }
        }

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
        public static void Register<TViewModel, TView>()
            where TViewModel : BaseViewModel
            where TView : Page
        {
            Register<TViewModel, TView>(args => (TViewModel)Activator.CreateInstance(typeof(TViewModel), args));
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
        public static void Register<TViewModel, TView>(TViewModel instance)
            where TViewModel : BaseViewModel
            where TView : Page
        {
            Register<TViewModel, TView>(_ => instance);
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
        public static void Register<TViewModel, TView>(Func<object[], TViewModel> @delegate)
            where TViewModel : BaseViewModel
            where TView : Page
        {
            var type = typeof(TViewModel);

            _viewModelMap.Add(type, typeof(TView));
            _viewModelGenerators.Add(type, @delegate);
        }

        /// <summary>
        /// Navigates the application to the view corresponding to the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the desired view model.</typeparam>
        /// <param name="args">An optional list of arguments that will be used to initialize the view model.</param>
        /// <returns>
        /// The instance of the view model that was used for navigation.
        /// </returns>
        /// <seealso cref="M:Register{TViewModel, TView}" />
        /// <exception cref="System.ArgumentException">The specified view model type has no registered view.</exception>
        /// <remarks>
        /// The requested view model type must be registered with the
        /// <see cref="M:Register{TViewModel, TView}" /> method before navigating.
        /// </remarks>
        public static TViewModel GoTo<TViewModel>(params object[] args)
            where TViewModel : BaseViewModel
        {
            var type = typeof(TViewModel);

            if (!_viewModelMap.ContainsKey(type))
            {
                throw new ArgumentException("The specified view model type has no registered view.");
            }

            var instance = _viewModelGenerators[type](args);

            return GoToInstance<TViewModel>((TViewModel)instance);
        }

        /// <summary>
        /// Navigates the application to the view corresponding to the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the desired view model.</typeparam>
        /// <param name="instance">The instance of the view model that will be passed on to the view.</param>
        /// <returns>
        /// The instance of the view model that was used for navigation.
        /// </returns>
        /// <seealso cref="M:Register{TViewModel, TView}" />
        /// <exception cref="System.ArgumentException">The specified view model type has no registered view.</exception>
        /// <exception cref="System.Exception">Failed to navigate to requested page.</exception>
        /// <remarks>
        /// The requested view model type must be registered with the
        /// <see cref="M:Register{TViewModel, TView}" /> method before navigating.
        /// </remarks>
        public static TViewModel GoToInstance<TViewModel>(TViewModel instance)
            where TViewModel : BaseViewModel
        {
            var type = instance.GetType();

            if (!_viewModelMap.ContainsKey(type))
            {
                throw new ArgumentException("The specified view model type has no registered view.");
            }

            _viewModels.Push(instance);

            Frame rootFrame = CurrentFrame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Place the frame in the current Window
                CurrentFrame = rootFrame;
            }

            if (!CurrentFrame.Navigate(_viewModelMap[type], instance))
            {
                throw new Exception("Failed to navigate to requested page.");
            }

            return instance;
        }

        /// <summary>
        /// Navigates the application to the view corresponding to the specified view model and activates
        /// the application.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the desired view model.</typeparam>
        /// <param name="args">An optional list of arguments that will be used to initialize the view model.</param>
        /// <returns>
        /// The instance of the view model that was used for navigation.
        /// </returns>
        /// <seealso cref="M:Register{TViewModel, TView}" />
        /// <seealso cref="M:GoTo{TViewModel}"/>
        /// <remarks>
        /// The requested view model type must be registered with the <see cref="M:Register{TViewModel, TView}" /> method before navigating.
        /// </remarks>
        public static TViewModel Activate<TViewModel>(params object[] args)
            where TViewModel : BaseViewModel
        {
            var result = GoTo<TViewModel>(args);

            Window.Current.Activate();

            return result;
        }
    }
}
