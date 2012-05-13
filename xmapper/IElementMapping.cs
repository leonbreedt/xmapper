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

namespace XMapper
{
    /// <summary>
    /// Base interface for element mappings.
    /// </summary>
    public interface IElementMapping : IMapping
    {
        /// <summary>
        /// Creates an instance of the type that this mapping is associated with.
        /// </summary>
        /// <returns>Returns the new instance</returns>
        object CreateInstance();

        /// <summary>
        /// Attempts to find an attribute mapping within this element mapping, having the given local name and no
        /// namespace.
        /// </summary>
        /// <param name="localName">The local name of the attribute.</param>
        /// <returns>Returns the attribute mapping if found, otherwise <c>null</c>.</returns>
        IAttributeMapping TryFindAttributeMapping(string localName);

        /// <summary>
        /// Attempts to find an attribute mapping within this element mapping, having the given local name and a
        /// namespace.
        /// </summary>
        /// <param name="namespaceUri">The namespace of the attribute.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <returns>Returns the attribute mapping if found, otherwise <c>null</c>.</returns>
        IAttributeMapping TryFindAttributeMapping(string namespaceUri, string localName);

        /// <summary>
        /// Attempts to find a child element mapping within this element mapping, having the given local name and a
        /// namespace.
        /// </summary>
        /// <param name="localName">The local name of the child element.</param>
        /// <returns>Returns the child element mapping if found, otherwise <c>null</c>.</returns>
        IChildElementMapping TryFindChildElementMapping(string localName);

        /// <summary>
        /// Attempts to find a child element mapping within this element mapping, having the given local name and a
        /// namespace.
        /// </summary>
        /// <param name="namespaceUri">The namespace of the child element.</param>
        /// <param name="localName">The local name of the child element.</param>
        /// <returns>Returns the child element mapping if found, otherwise <c>null</c>.</returns>
        IChildElementMapping TryFindChildElementMapping(string namespaceUri, string localName);

        /// <summary>
        /// Attempts to find a child text element mapping within this element mapping, having the given local name and no
        /// namespace.
        /// </summary>
        /// <param name="localName">The local name of the child element.</param>
        /// <returns>Returns the child element mapping if found, otherwise <c>null</c>.</returns>
        ITextContentMapping TryFindChildTextElementMapping(string localName);

        /// <summary>
        /// Attempts to find a child text element mapping within this element mapping, having the given local name and a
        /// namespace.
        /// </summary>
        /// <param name="namespaceUri">The namespace of the child element.</param>
        /// <param name="localName">The local name of the child element.</param>
        /// <returns>Returns the child element mapping if found, otherwise <c>null</c>.</returns>
        ITextContentMapping TryFindChildTextElementMapping(string namespaceUri, string localName);
    }
}
