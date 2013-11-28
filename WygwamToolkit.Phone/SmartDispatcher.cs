//-----------------------------------------------------------------------
// <copyright file="SmartDispatcher.cs" company="Wygwam">
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
    using global::Windows.ApplicationModel;
    using global::Windows.UI.Core;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// TODO: Provide summary section in the documentation header.
    /// </summary>
    public class SmartDispatcher
    {
        private static int uiThreadId;
        private static Dispatcher _dispatcher;

        public static bool IsDesigner
        {
            get { return System.ComponentModel.DesignerProperties.IsInDesignTool; }
        }

        public static event EventHandler Initialized;

        public static bool IsInitialized
        {
            get
            {
                return _dispatcher != null;
            }
        }

        public static void Initialize()
        {
            _dispatcher = Deployment.Current.Dispatcher;
            uiThreadId = Thread.CurrentThread.ManagedThreadId;

            if (Initialized != null)
                Initialized(_dispatcher, null);
        }

        public static bool CanExecuteOnCurrentThread()
        {
            RequireInstance();

            return Thread.CurrentThread.ManagedThreadId == uiThreadId;
        }

        public static async Task BeginInvoke(Action action)
        {
            RequireInstance();

            // If the current thread is the user interface thread, skip the
            // dispatcher and directly invoke the Action.
            if (CanExecuteOnCurrentThread() || IsDesigner)
            {
                action();
            }
            else
            {
                TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
                _dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            action();
                            task.SetResult(true);
                        }
                        catch (Exception ex)
                        {
                            task.SetException(ex);
                        }
                    });
                await task.Task;
            }
        }

        private static void RequireInstance()
        {
            if (_dispatcher == null)
            {
                throw new InvalidOperationException("SmartDispatcher must be initialized before it is used.");
            }
        }
    }
}
