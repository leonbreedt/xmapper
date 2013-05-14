//
// Copyright (C) 2010-2012 Leon Breedt
// ljb -at- bitserf [dot] org
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

using System.Collections;
using System.Xml;

namespace XMapper
{
    /// <summary>
    /// Represents a mapping of an XML any elements to an object property. That is, any XML element
    /// that is not explicitly declared to be supported will be mapped to the specified property. The 
    /// property must be of type 
    /// </summary>
    public interface IAnyElementMapping
    {
        /// <summary>
        /// Deserializes an element from an XmlReader.
        /// </summary>
        /// <param name="reader">The reader to use, must be positioned on an element.</param>
        object DeserializeElement(XmlReader reader);

        /// <summary>
        /// Serializes an element to an XmlWriter.
        /// </summary>
        /// <param name="writer">The writer, must be positioned on an element.</param>
        /// <param name="element">The element to serialize.</param>
        void SerializeElement(XmlWriter writer, object element);

        /// <summary>
        /// Gets the custom elements currently stored against the target.
        /// </summary>
        /// <param name="target">The target object.</param>
        IList GetElements(object target);

        /// <summary>
        /// Adds the specified element to the list of custom elements on the target.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="element">The element to add.</param>
        void AddToElements(object target, object element);
    }
}
