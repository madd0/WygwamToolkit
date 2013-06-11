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
    using global::Windows.UI.Xaml;
    using System;

    /// <summary>
    /// TODO: Provide summary section in the documentation header.
    /// </summary>
    public class SmartDispatcher
    {
        private static CoreDispatcher _dispatcher;

        public static bool IsDesigner
        {
            get { return DesignMode.DesignModeEnabled; }
        }

        public static bool IsInitialized
        {
            get
            {
                return _dispatcher != null;
            }
        }

        public static void Initialize()
        {
            _dispatcher = Window.Current.CoreWindow.Dispatcher;
        }

        public static bool CanExecuteOnCurrentThread()
        {
            RequireInstance();

            return _dispatcher.HasThreadAccess;
        }

        public static void BeginInvoke(Action action)
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
                _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
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
