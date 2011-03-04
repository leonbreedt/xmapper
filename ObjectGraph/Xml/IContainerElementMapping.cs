﻿//
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

using System.Collections.Generic;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Represents a mapping of an XML container element to a CLR type.
    /// </summary>
    /// <typeparam name="TContainingTarget">The CLR type that this mapping is contained within.</typeparam>
    /// <typeparam name="TMemberTarget">The CLR type associated with this members of this container mapping.</typeparam>
    public interface IContainerElementMapping<TContainingTarget, TMemberTarget> : IElementMapping<IList<TMemberTarget>>
    {
        /// <summary>
        /// Gets the collection from the specified object.
        /// </summary>
        /// <param name="container">The object containing the collection on a property.</param>
        /// <returns>Returns the collection.</returns>
        IList<TMemberTarget> GetCollectionFromTarget(TContainingTarget container);

        /// <summary>
        /// Sets the collection on the specified object.
        /// </summary>
        /// <param name="container">The object containing the collection on a property.</param>
        /// <param name="collection">The collection to set.</param>
        void SetCollectionOnTarget(TContainingTarget container, IList<TMemberTarget> collection);
    }
}