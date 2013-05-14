//-----------------------------------------------------------------------
// <copyright file="IListExtensions.cs" company="Wygwam">
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
    using System.Collections.Generic;

    /// <summary>
    /// Provides extension methods for <see cref="System.Collections.Generic.IList{T}"/>.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Adds a range of items to a list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to which items will be added.</param>
        /// <param name="range">The range of items to add.</param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                list.Add(item);
            }
        }
    }
}
