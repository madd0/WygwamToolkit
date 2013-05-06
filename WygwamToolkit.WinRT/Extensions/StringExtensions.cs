//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Wygwam">
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
    using global::Windows.UI;
    using System.Text.RegularExpressions;
    using System.Globalization;

    /// <summary>
    /// Provides extensions methods for <see cref="System.String"/>.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Regex regex = new Regex("#(?<alpha>[0-9a-f]{2})?(?<red>[0-9a-f]{2})(?<green>[0-9a-f]{2})(?<blue>[0-9a-f]{2})", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        /// <summary>
        /// Converts an aRGB hex string to a <see cref="Windows.UI.Color"/>.
        /// </summary>
        /// <param name="str">The aRGB hex string.</param>
        /// <returns>The <see cref="Windows.UI.Color"/> corresponding to the provided aRGB string.</returns>
        public static Color AsColor(this string str)
        {
            var matches = regex.Match(str);

            if (!matches.Success)
            {
                throw new ArgumentOutOfRangeException("str", "The provided string is not an aRGB hex color.");
            }

            return Color.FromArgb(
                matches.Groups["alpha"].Success ? byte.Parse(matches.Groups["alpha"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture) : (byte)0xFF,
                byte.Parse(matches.Groups["red"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                byte.Parse(matches.Groups["green"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                byte.Parse(matches.Groups["blue"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture)
            );
        }
    }
}
