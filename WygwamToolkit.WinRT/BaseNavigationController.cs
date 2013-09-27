//-----------------------------------------------------------------------
// <copyright file="BaseNavigationController.cs" company="Wygwam">
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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Wygwam.Windows.ViewModels;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
#elif WINDOWS_PHONE
    using System.Windows;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Provides MVVM-compatible navigation for Windows Store and Windows Phone apps.
    /// </summary>
    public abstract class BaseNavigationController
    {
        private readonly Dictionary<Type, Func<object[], BaseViewModel>> _viewModelGenerators = new Dictionary<Type, Func<object[], BaseViewModel>>();

        private readonly Stack<BaseViewModel> _viewModels = new Stack<BaseViewModel>();

        private IWindowManager _windowManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationController"/> class.
        /// </summary>
        public BaseNavigationController()
            : this(new DefaultWindowManager())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationController"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager responsible for providing the <see cref="global::Windows.UI.Xaml.Controls.Frame"/>
        /// used for navigation.</param>
        public BaseNavigationController(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        /// <summary>
        /// Gets the current view model.
        /// </summary>
        public BaseViewModel CurrentViewModel
        {
            get
            {
                return this.ViewModelNavigationStack.Peek() as BaseViewModel;
            }
        }

        public Page CurrentView
        {
            get
            {
                return _windowManager.NavigationFrame.Content as Page;
            }
        }

        protected IWindowManager WindowManager
        {
            get { return _windowManager; }

            set { _windowManager = value; }
        }

        protected Stack<BaseViewModel> ViewModelNavigationStack
        {
            get { return _viewModels; }
        }

        /// <summary>
        /// Goes back in the navigation stack.
        /// </summary>
        public void GoBack()
        {
            if (this.CanGoBack)
            {
                IDisposable oldViewModel = _viewModels.Pop() as IDisposable;

                var currentPage = this.GoBackInternal();

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

                if (oldViewModel != null)
                {
                    oldViewModel.Dispose();
                }
            }
        }

        public virtual bool CanGoBack
        {
            get
            {
                var frame = this.WindowManager.NavigationFrame;

                return (frame != null && frame.CanGoBack);
            }
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
        public TViewModel GoTo<TViewModel>(params object[] args)
            where TViewModel : BaseViewModel
        {
            var type = typeof(TViewModel);

            if (!this.IsViewModelRegistered(type))
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
        public TViewModel GoToInstance<TViewModel>(TViewModel instance)
            where TViewModel : BaseViewModel
        {
            var type = instance.GetType();

            if (!this.IsViewModelRegistered(type))
            {
                throw new ArgumentException("The specified view model type has no registered view.");
            }

            if (this.NavigateToType(type, instance))
            {
                _viewModels.Push(instance);
            }
            else
            {
                throw new Exception("Failed to navigate to requested page.");
            }

            return instance;
        }

        public BaseViewModel GoToType(Type viewModelType, params object[] args)
        {
            if (!this.IsViewModelRegistered(viewModelType))
            {
                throw new ArgumentException("The specified view model type has no registered view.");
            }

            var instance = _viewModelGenerators[viewModelType](args);

            if (this.NavigateToType(viewModelType , instance))
            {
                _viewModels.Push(instance);
            }
            else
            {
                throw new Exception("Failed to navigate to requested page.");
            }

            return instance;
        }

        protected virtual FrameworkElement GoBackInternal()
        {
            var frame = this.WindowManager.NavigationFrame;

            frame.GoBack();

            return frame.Content as FrameworkElement;
        }

        protected abstract bool NavigateToType(Type viewModelType, object viewModelInstance);

        protected abstract bool IsViewModelRegistered(Type viewModelType);

        protected void RegisterViewModelGenerator<TViewModel>(Type viewModelType, Func<object[], TViewModel> generator)
            where TViewModel : BaseViewModel
        {
            _viewModelGenerators.Add(viewModelType, generator);
        }
    }
}
