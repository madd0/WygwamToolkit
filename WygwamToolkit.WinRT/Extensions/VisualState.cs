//-----------------------------------------------------------------------
// <copyright file="VisualState.cs" company="Wygwam">
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
namespace Wygwam.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
#elif WINDOWS_PHONE
    using System.Windows;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Provides attached properties that allow the transitioning of a <see cref="global::Windows.UI.Xaml.Controls.Control"/>'s visual state
    /// through binding.
    /// </summary>
    public static class VisualState
    {
        /// <summary>
        /// Identifies the VisualState attached property.
        /// </summary>
        public static readonly DependencyProperty VisualStateProperty =
            DependencyProperty.RegisterAttached("VisualState", typeof(string), typeof(VisualState), new PropertyMetadata(null, OnVisualStateChanged));

        /// <summary>
        /// Identifies the UseVisualStateTransitions attached property.
        /// </summary>
        public static readonly DependencyProperty UseVisualStateTransitionsProperty =
            DependencyProperty.RegisterAttached("UseVisualStateTransitions", typeof(bool), typeof(VisualState), new PropertyMetadata(true));

        /// <summary>
        /// Gets the visual state of a <see cref="global::Windows.UI.Xaml.Controls.Control"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="global::Windows.UI.Xaml.DependencyObject"/> to which the property is attached. This must be a
        /// <see cref="global::Windows.UI.Xaml.Controls.Control"/>.</param>
        /// <returns>The name of the visual state as a string.</returns>
        /// <remarks>The attached property does not detect changes in the visual state of the attached <see cref="global::Windows.UI.Xaml.Controls.Control"/>
        /// that happen outside its scope. Therefore, the value read using this attached property cannot be considered reliable.</remarks>
        public static string GetVisualState(DependencyObject obj)
        {
            return (string)obj.GetValue(VisualStateProperty);
        }

        /// <summary>
        /// Sets the visual state of a <see cref="global::Windows.UI.Xaml.Controls.Control"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="global::Windows.UI.Xaml.DependencyObject"/> to which the property is attached. This must be a
        /// <see cref="global::Windows.UI.Xaml.Controls.Control"/>.</param>
        /// <param name="value">The name of the new state of the <see cref="global::Windows.UI.Xaml.Controls.Control"/>.</param>
        public static void SetVisualState(DependencyObject obj, string value)
        {
            obj.SetValue(VisualStateProperty, value);
        }

        /// <summary>
        /// Gets a value that determines whether transitions should be used when the visual state of the attached
        /// <see cref="global::Windows.UI.Xaml.Controls.Control"/> changes.
        /// </summary>
        /// <param name="obj">The instance of <see cref="global::Windows.UI.Xaml.DependencyObject"/> to which the property is attached. This must be a
        /// <see cref="global::Windows.UI.Xaml.Controls.Control"/>.</param>
        /// <returns><c>true</c> if transitions should be used when the visual state of the control changes; otherwise <c>false</c>.</returns>
        /// <remarks>This value is used by the method that handles changes in the VisualState attached property.</remarks>
        public static bool GetUseVisualStateTransitions(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseVisualStateTransitionsProperty);
        }

        /// <summary>
        /// Sets a value that determines whether transitions should be used when the visual state of the attached
        /// <see cref="global::Windows.UI.Xaml.Controls.Control"/> changes.
        /// </summary>
        /// <param name="obj">The instance of <see cref="global::Windows.UI.Xaml.DependencyObject"/> to which the property is attached. This must be a
        /// <see cref="global::Windows.UI.Xaml.Controls.Control"/>.</param>
        /// <param name="value">if set to <c>true</c>, transitions should be used when the visual state of the control changes.</param>
        public static void SetUseVisualStateTransitions(DependencyObject obj, bool value)
        {
            obj.SetValue(UseVisualStateTransitionsProperty, value);
        }

        /// <summary>
        /// Called when the visual state of a <see cref="global::Windows.UI.Xaml.DependencyObject"/> is changed using
        /// the attached property.
        /// </summary>
        /// <param name="d">The instance of <see cref="global::Windows.UI.Xaml.DependencyObject"/> on which the attached
        /// property was changed.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisualStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Control;
            var state = e.NewValue as string;

            if (control != null && !string.IsNullOrWhiteSpace(state))
            {
                VisualStateManager.GoToState(control, state, GetUseVisualStateTransitions(d));
            }
        }
    }
}
