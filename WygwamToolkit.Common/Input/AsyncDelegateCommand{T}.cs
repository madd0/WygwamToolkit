//-----------------------------------------------------------------------
// <copyright file="AsyncDelegateCommand{T}.cs" company="Wygwam">
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
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
    /// The delegates passed to this class support command parameters.
    /// </summary>
    /// <typeparam name="T">The of the parameter passed to the delegates when the command is invoked.</typeparam>
    public class AsyncDelegateCommand<T> : ICommand
    {
        private Func<T, bool> _canExecute;
        private Func<T, Task> _execute;

        private bool _isRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">The delegate that will be called when the command is invoked.</param>
        /// <param name="canExecute">A predicate that will determine whether the command can be invoked.</param>
        public AsyncDelegateCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute ?? (_ => true);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, 
        /// this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return !_isRunning && _canExecute((T)parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, 
        /// this object can be set to null.</param>
        public async void Execute(object parameter)
        {
            await this.ExecuteAsync(parameter);
        }

        /// <summary>
        /// Defines the method to be called asynchronously when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed,
        /// this object can be set to null.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> to help with asynchronous programming.</returns>
        public async Task ExecuteAsync(object parameter)
        {
            _isRunning = true;

            try
            {
                this.RaiseCanExecuteChanged();
                await _execute((T)parameter);
            }
            finally
            {
                _isRunning = false;
                this.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Raises the can execute change.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="E:CanExecuteChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void OnCanExecuteChanged(EventArgs e)
        {
            var handler = this.CanExecuteChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
