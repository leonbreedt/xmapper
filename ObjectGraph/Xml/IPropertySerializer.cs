//
// Copyright (C) 2010 Leon Breedt
// bitserf -at- gmail [dot] com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.using System;
//

using System.Xml;
using System.Xml.Linq;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Enumerates the types of serializer type.
    /// </summary>
    internal enum PropertySerializerType
    {
        Simple,
        Complex
    }

    /// <summary>
    /// Defines the contract for a serializer of properties.
    /// </summary>
    internal interface IPropertySerializer<T>
        where T : new()
    {
        /// <summary>
        /// Gets the name to use for the property when serializing it.
        /// </summary>
        XName Name { get; }

        /// <summary>
        /// Gets the serializer type.
        /// </summary>
        PropertySerializerType Type { get; }

        /// <summary>
        /// Reads the property value from the specified reader, and applies it to the target object.
        /// </summary>
        /// <param name="reader">The XML reader, currently positioned on an attribute value.</param>
        /// <param name="target">The target object to set the property value on.</param>
        void ReadProperty(XmlReader reader, T target);

        /// <summary>
        /// Reads the current property value from the source object, and writes it to the specified XML
        /// writer.
        /// </summary>
        /// <param name="writer">The XML writer to write the property to.</param>
        /// <param name="source">The source object to read the current property value from.</param>
        void WriteProperty(XmlWriter writer, T source);
    }
}
