//-----------------------------------------------------------------------
// <copyright file="XmlDataSerializer.cs" company="Wygwam">
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
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// An implementation of <see cref="IDataSerializer"/> that uses the
    /// <see cref="System.Xml.Serialization.XmlSerializer"/> for serialization.
    /// </summary>
    public class XmlDataSerializer : IDataSerializer
    {
        /// <summary>
        /// Serializes an object to the specified stream.
        /// </summary>
        /// <param name="stream">The stream where the data will be serialized.</param>
        /// <param name="data">The data to serialize.</param>
        public void Serialize(Stream stream, object data)
        {
            var serializer = new XmlSerializer(data.GetType());

            serializer.Serialize(stream, data);
        }

        /// <summary>
        /// Deserializes an object from the specified stream.
        /// </summary>
        /// <typeparam name="T">The type of the object that is being deserialized.</typeparam>
        /// <param name="stream">The stream where the serialized data will be read.</param>
        /// <returns>
        /// The object deserialized from the provided stream.
        /// </returns>
        public T Deserialize<T>(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(T));

            return (T)serializer.Deserialize(stream);
        }
    }
}
