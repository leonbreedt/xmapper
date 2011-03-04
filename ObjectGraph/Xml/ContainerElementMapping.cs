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
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using ObjectGraph.Util;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Represents a mapping of an XML child element to a CLR type.
    /// </summary>
    /// <typeparam name="TContainingTarget">The CLR type that contains the property this mapping will be associated with.</typeparam>
    /// <typeparam name="TMemberTarget">The CLR type associated with a member of this element mapping.</typeparam>
    public class ContainerElementMapping<TContainingTarget, TMemberTarget> : ElementMapping<ItemCollection<TMemberTarget>>, IContainerElementMapping<TContainingTarget, TMemberTarget>
    {
        #region Fields
        readonly PropertyInfo _propertyInfo;
        readonly Func<TContainingTarget, ItemCollection<TMemberTarget>> _getter;
        readonly Action<TContainingTarget, ItemCollection<TMemberTarget>> _setter;
        #endregion

        /// <summary>
        /// Creates a new child element mapping.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="propertyExpression">A simple member expression referencing the property in the container to associate this mapping with.</param>
        public ContainerElementMapping(XName name, Expression<Func<TContainingTarget, ItemCollection<TMemberTarget>>> propertyExpression)
            : base(name)
        {
            _propertyInfo = ReflectionHelper.GetPropertyInfoFromExpression(propertyExpression);
            _getter = ReflectionHelper.GetPropertyGetterDelegate<TContainingTarget, ItemCollection<TMemberTarget>>(_propertyInfo);
            _setter = ReflectionHelper.GetPropertySetterDelegate<TContainingTarget, ItemCollection<TMemberTarget>>(_propertyInfo);
        }

        public ItemCollection<TMemberTarget> GetCollectionFromTarget(TContainingTarget container)
        {
            return _getter(container);
        }

        public void SetCollectionOnTarget(TContainingTarget container, ItemCollection<TMemberTarget> collection)
        {
            _setter(container, collection);
        }

        public override ItemCollection<TMemberTarget> CreateInstance()
        {
            return new ItemCollection<TMemberTarget>();
        }
    }
}
