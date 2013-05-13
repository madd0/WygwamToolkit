//-----------------------------------------------------------------------
// <copyright file="SmartDispatcher.cs" company="">
//     Copyright (c) mauri_000, . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Wygwam.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Windows.ApplicationModel;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;


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

        private static bool IsInitialized()
        {
            RequireInstance();

            return _dispatcher != null;
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
