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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using XMapper.Util;

namespace XMapper
{
    /// <summary>
    /// Represents a mapping of an XML any attribute to an object property. That is, any XML attribute
    /// that is not explicitly declared to be supported will be mapped to the specified property.
    /// </summary>
    /// <typeparam name="TContainer">The type that contains the property this mapping is associated with.</typeparam>
    public class AnyAttributeMapping<TContainer> : AnyMappingBase<TContainer, XAttribute>, IAnyAttributeMapping
    {
        /// <summary>
        /// Creates a new XML attribute mapping for custom attributes.
        /// </summary>
        /// <param name="propertyExpression">A simple member expression referencing the property to associate this mapping with.</param>
        public AnyAttributeMapping(Expression<Func<TContainer, IList<XAttribute>>> propertyExpression)
            : base(propertyExpression)
        {
        }

        public IList GetAttributes(object target)
        {
            return GetCustomNodeList(target);
        }

        public void AddToAttributes(object target, XAttribute attribute)
        {
            AddToCustomNodeList(target, attribute);
        }
    }
}