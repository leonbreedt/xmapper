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
using XMapper.Util;

namespace XMapper
{
    /// <summary>
    /// Represents a mapping of an XML child element to a regular property on a type.
    /// </summary>
    /// <typeparam name="TContainer">The  type that contains the property that will be read and written.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    public class ChildElementMapping<TContainer, TProperty> : ElementMapping<TProperty>, IChildElementMapping
    {
        #region Fields
        readonly PropertyInfo _propertyInfo;
        readonly Func<TContainer, TProperty> _getter;
        readonly Action<TContainer, TProperty> _setter;
        #endregion

        /// <summary>
        /// Creates a new child element mapping.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="propertyExpression">A simple member expression referencing the property that will be read and written.</param>
        public ChildElementMapping(XName name, Expression<Func<TContainer, TProperty>> propertyExpression)
            : base(name)
        {
            if (propertyExpression != null)
            {
                _propertyInfo = ReflectionHelper.GetPropertyInfoFromExpression(propertyExpression);
                _getter = ReflectionHelper.GetTypedPropertyGetterDelegate<TContainer, TProperty>(_propertyInfo);
                _setter = ReflectionHelper.GetTypedPropertySetterDelegate<TContainer, TProperty>(_propertyInfo);
            }
        }

        public object GetFromContainer(object target)
        {
            if (_getter == null)
                throw new InvalidOperationException(
                    string.Format("Unable to get value for property {0} from container {1}, no getter is available.",
                                  typeof(TProperty),
                                  typeof(TContainer)));
            return _getter((TContainer)target);
        }

        public void SetOnContainer(object target, object item)
        {
            if (_setter == null)
                throw new InvalidOperationException(
                    string.Format("Unable to set value for property {0} on container {1}, no setter is available.",
                                  typeof(TProperty),
                                  typeof(TContainer)));
            _setter((TContainer)target, (TProperty)item);
        }
    }
}
