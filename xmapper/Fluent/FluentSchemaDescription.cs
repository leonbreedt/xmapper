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
using System.Linq;
using System.Xml.Linq;

namespace XMapper.Fluent
{
    /// <summary>
    /// Fluent interface for building a schema description.
    /// </summary>
    public class FluentSchemaDescription
    {
        #region Fields
        readonly List<Func<IElementMapping>> _rootMappingBuilders;
        #endregion

        /// <summary>
        /// Creates a new fluent schema description builder.
        /// </summary>
        public FluentSchemaDescription()
        {
            _rootMappingBuilders = new List<Func<IElementMapping>>();
        }

        /// <summary>
        /// Defines a root element.
        /// </summary>
        /// <typeparam name="TTarget">The element type.</typeparam>
        /// <param name="name">The element XML name.</param>
        /// <returns></returns>
        public IElementMappingBuilder<TTarget> Element<TTarget>(XName name)
        {
            var builder = new ElementMappingBuilder<TTarget>(name);
            _rootMappingBuilders.Add(builder.Build);
            return builder;
        }

        /// <summary>
        /// Builds the schema description.
        /// </summary>
        /// <returns>Returns the built schema description.</returns>
        public SchemaDescription Build()
        {
            var description = new SchemaDescription();

            foreach (var mapping in _rootMappingBuilders.Select(f => f()))
                AddToDescriptionRecursive(description, mapping);

            return description;
        }

        static void AddToDescriptionRecursive(SchemaDescription description, IElementMapping mapping)
        {
            description.Add(mapping);
            foreach (var childMapping in mapping.ChildElements)
                AddToDescriptionRecursive(description, childMapping);
        }
    }
}
