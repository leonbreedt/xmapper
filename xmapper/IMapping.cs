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

using System;

namespace XMapper
{
    /// <summary>
    /// Represents a mapping from an XML construct to a model element such as a class or class property.
    /// </summary>
    public interface IMapping
    {
        /// <summary>
        /// Gets the type associated with this mapping.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the XML namespace URI associated with the XML construct being mapped from.
        /// </summary>
        string NamespaceUri { get; }

        /// <summary>
        /// Gets the XML local name associated with the XML construct being mapped from.
        /// </summary>
        string LocalName { get; }

        /// <summary>
        /// Gets the attribute mappings contained within this mapping, if any.
        /// </summary>
        IAttributeMapping[] Attributes { get; }

        /// <summary>
        /// Gets the custom attribute catch-all mapping, if any.
        /// </summary>
        IAnyAttributeMapping AnyAttribute { get; }

        /// <summary>
        /// Gets the mappings for the child elements contained within this mapping, if any.
        /// </summary>
        IChildElementMapping[] ChildElements { get; }

        /// <summary>
        /// Gets the custom element catch-all mapping, if any.
        /// </summary>
        IAnyElementMapping AnyChildElement { get; }

        /// <summary>
        /// Gets the text content mapping for this element.
        /// </summary>
        ITextContentMapping TextContent { get; }

        /// <summary>
        /// Gets the any child text element mappings for this element.
        /// </summary>
        ITextContentMapping[] ChildTextElements { get; }
    }
}
