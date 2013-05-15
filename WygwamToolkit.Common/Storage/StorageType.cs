//-----------------------------------------------------------------------
// <copyright file="StorageType.cs" company="Wygwam">
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

namespace Wygwam.Windows.Storage
{
    /// <summary>
    /// Provides values that identify the different storage types.
    /// </summary>
    public enum StorageType
    {
        /// <summary>
        /// Storage on the client machine.
        /// </summary>
        Local,

        /// <summary>
        /// Storage in a centralized location that is synchronized on different machines.
        /// </summary>
        Roaming,

        /// <summary>
        /// Temporary storage on the client machine.
        /// </summary>
        Temp
    }
}
