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

using System;
using System.Collections.Generic;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Represents collection of an XML element mappings.
    /// </summary>
    public class SchemaDescription
    {
        #region Fields
        readonly Dictionary<Type, IMapping> _elementMappingsByType;
        #endregion

        /// <summary>
        /// Creates a new mapping collection.
        /// </summary>
        internal SchemaDescription()
        {
            _elementMappingsByType = new Dictionary<Type, IMapping>();
        }

        /// <summary>
        /// Looks up an XML element mapping.
        /// </summary>
        /// <typeparam name="T">The CLR type to look up a mapping for.</typeparam>
        /// <returns>Returns the mapping for the type, or <c>null</c> if no mapping exists.</returns>
        public IMapping TryFindMappingForType<T>()
        {
            IMapping mapping;
            if (_elementMappingsByType.TryGetValue(typeof(T), out mapping))
                return mapping;
            return null;
        }

        internal void Add(IMapping mapping)
        {
            _elementMappingsByType[mapping.Type] = mapping;
        }
    }
}
