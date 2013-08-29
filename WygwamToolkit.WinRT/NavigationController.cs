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
    public class NavigationController : BaseNavigationController
    {
        private readonly Dictionary<Type, Type> _viewModelMap = new Dictionary<Type, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationController"/> class.
        /// </summary>
        public NavigationController()
            : base(new DefaultWindowManager())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationController"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager responsible for providing the <see cref="global::Windows.UI.Xaml.Controls.Frame"/>
        /// used for navigation.</param>
        public NavigationController(IWindowManager windowManager)
            : base(windowManager)
        { }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        public Page CurrentView
        {
            get
            {
                var frame = this.WindowManager.NavigationFrame;

                return frame != null ? frame.Content as Page : null;
            }
        }

        protected override bool IsViewModelRegistered(Type viewModelType)
        {
            return _viewModelMap.ContainsKey(viewModelType);
        }

        protected override bool NavigateToType(Type viewModelType, object viewModelInstance)
        {
            var frame = this.WindowManager.EnsureNavigationFrameExists();

            return frame.Navigate(_viewModelMap[viewModelType], viewModelInstance);
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
        public void Register<TViewModel, TView>()
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
        public void Register<TViewModel, TView>(TViewModel instance)
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
        public void Register<TViewModel, TView>(Func<object[], TViewModel> @delegate)
            where TViewModel : BaseViewModel
            where TView : Page
        {
            var type = typeof(TViewModel);

            _viewModelMap.Add(type, typeof(TView));

            this.RegisterViewModelGenerator<TViewModel>(type, @delegate);
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
        public TViewModel Activate<TViewModel>(params object[] args)
            where TViewModel : BaseViewModel
        {
            var result = GoTo<TViewModel>(args);

            Window.Current.Activate();

            return result;
        }

        /// <summary>
        /// Sets the instance of <see cref="IWindowManager" /> that will be used to access the
        /// <see cref="global::Windows.UI.Xaml.Controls.Frame" /> used for navigation.
        /// </summary>
        /// <param name="manager">An instance of <see cref="IWindowManager" /> that provides acces to the navigation frame.</param>
        /// <seealso cref="DefaultWindowManager" />
        /// <exception cref="System.ArgumentNullException">manager;The window manager cannot be null.</exception>
        /// <remarks>
        /// If no window manager is set, the controller uses and instance of <see cref="DefaultWindowManager" />.
        /// </remarks>
        public void SetWindowManager(IWindowManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager", "The window manager cannot be null.");
            }

            this.WindowManager = manager;
        }
    }
}
