//
// Copyright (C) 2010-2011 Leon Breedt
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

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Base interface for collection child element mappings.
    /// </summary>
    public interface ICollectionChildElementMapping : IChildElementMapping
    {
        /// <summary>
        /// Whether or not the associated container is a collection itself.
        /// </summary>
        bool IsContainerCollection { get; }

        /// <summary>
        /// Adds the specified member to the collection property of the containing object. If the property
        /// is not initialized, a new collection will be created.
        /// </summary>
        /// <param name="container">The object containing the collection property.</param>
        /// <param name="member">The member to add to the collection.</param>
        void AddToCollection(object container, object member);

        /// <summary>
        /// Gets the collection from the containing object.
        /// </summary>
        /// <param name="container">The object containing the collection property.</param>
        IList GetCollection(object container);
    }

    /// <summary>
    /// Represents a mapping of an XML child element onto a collection property of a containing object.
    /// </summary>
    /// <typeparam name="TContainer">The type of the class containing the collection property.</typeparam>
    /// <typeparam name="TMember">The type of a member of the collection.</typeparam>
    public interface ICollectionChildElementMapping<TContainer, TMember> : IElementMapping<TMember>, ICollectionChildElementMapping
    {
    }
}
