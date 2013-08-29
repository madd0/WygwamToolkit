//-----------------------------------------------------------------------
// <copyright file="DefaultWindowManager.cs" company="Wygwam">
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
    using Microsoft.Phone.Controls;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// An implementation of <see cref="T:IWindowManager"/> that provides the content
    /// of the app window as a <see cref="global::Windows.UI.Xaml.Controls.Frame"/> or sets
    /// a new <see cref="global::Windows.UI.Xaml.Controls.Frame"/> if necessary.
    /// </summary>
    public class DefaultWindowManager : IWindowManager
    {
        /// <summary>
        /// Gets the <see cref="global::Windows.UI.Xaml.Controls.Frame" /> used for navigation if it exists.
        /// </summary>
        /// <remarks>
        /// Use <see cref="M:EnsureNaviationFrameExists" /> if you need to be sure that a frame for
        /// navigation exits (e.g. when the application navigates to its first page.)
        /// </remarks>
        public Frame NavigationFrame
        {
            get
            {
                return Application.Current.RootVisual as Frame;
            }

            private set
            {
                Application.Current.RootVisual = value;
            }
        }

        /// <summary>
        /// Ensures the navigation frame exists.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// Implementation of this method should check that a frame is available that can be used for
        /// navigation. If it isn't, it should take the necessary steps to create one and set the contents of
        /// the current <see cref="global::Windows.UI.Xaml.Window" /> appropriately.
        /// </remarks>
        public Frame EnsureNavigationFrameExists()
        {
            if (this.NavigationFrame == null)
            {
                // Create a Frame to act as the navigation context and set it as window content
                this.NavigationFrame = new PhoneApplicationFrame();
            }

            return this.NavigationFrame;
        }
    }
}
