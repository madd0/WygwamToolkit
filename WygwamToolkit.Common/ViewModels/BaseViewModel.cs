//-----------------------------------------------------------------------
// <copyright file="BaseViewModel.cs" company="Wygwam">
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

namespace Wygwam.Windows.ViewModels
{
    using System.Threading.Tasks;

    /// <summary>
    /// Serves as a base for view model classes.
    /// </summary>
    public class BaseViewModel : BindableBase
    {
        private static readonly Task<bool> _defaultTask = Task.FromResult(false);

        /// <summary>
        /// Called when the underlying view has finished loading.
        /// </summary>
        /// <returns>An instance of <see cref="System.Threading.Tasks.Task"/> that helps in
        /// asynchronous programming.</returns>
        public virtual Task OnLoaded()
        {
            return _defaultTask;
        }

        /// <summary>
        /// Called when the application is being suspended.
        /// </summary>
        /// <returns>An instance of <see cref="System.Threading.Tasks.Task"/> that helps in
        /// asynchronous programming.</returns>
        public virtual Task OnSuspending()
        {
            return _defaultTask;
        }

        /// <summary>
        /// Called when the application is being resumed.
        /// </summary>
        /// <returns>An instance of <see cref="System.Threading.Tasks.Task"/> that helps in
        /// asynchronous programming.</returns>
        public virtual Task OnResuming()
        {
            return _defaultTask;
        }

        /// <summary>
        /// Reloads this instance.
        /// </summary>
        /// <returns>An instance of <see cref="System.Threading.Tasks.Task"/> that helps in
        /// asynchronous programming.</returns>
        public virtual Task Reload()
        {
            return _defaultTask;
        }

        /// <summary>
        /// Called when the underlying view is about to leave the screen.
        /// </summary>
        /// <returns>An instance of <see cref="T:System.Threading.Tasks.Task{bool}"/> that, when completed,
        /// provides a result of <c>true</c> if the action must be cancelled, otherwise, a result of
        /// <c>false</c> will allow the view to close.</returns>
        public virtual Task<bool> OnLeaving()
        {
            return _defaultTask;
        }

        /// <summary>
        /// Called when the underlying view has left the screen.
        /// </summary>
        /// <returns>An instance of <see cref="System.Threading.Tasks.Task"/> that helps in
        /// asynchronous programming.</returns>
        public virtual Task OnLeft()
        {
            return _defaultTask;
        }
    }
}
