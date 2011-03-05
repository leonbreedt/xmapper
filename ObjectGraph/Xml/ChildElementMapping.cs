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
    /// <typeparam name="TTarget">The CLR type associated with this element mapping.</typeparam>
    public class ChildElementMapping<TContainingTarget, TTarget> : ElementMapping<TTarget>, IChildElementMapping<TContainingTarget, TTarget>
    {
        #region Fields
        readonly PropertyInfo _propertyInfo;
        readonly Func<TContainingTarget, TTarget> _getter;
        readonly Action<TContainingTarget, TTarget> _setter;
        #endregion

        /// <summary>
        /// Creates a new child element mapping.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="propertyExpression">A simple member expression referencing the property in the container to associate this mapping with.</param>
        public ChildElementMapping(XName name, Expression<Func<TContainingTarget, TTarget>> propertyExpression)
            : base(name)
        {
            if (propertyExpression != null)
            {
                _propertyInfo = ReflectionHelper.GetPropertyInfoFromExpression(propertyExpression);
                _getter = ReflectionHelper.GetTypedPropertyGetterDelegate<TContainingTarget, TTarget>(_propertyInfo);
                _setter = ReflectionHelper.GetTypedPropertySetterDelegate<TContainingTarget, TTarget>(_propertyInfo);
            }
        }

        public TTarget GetFromContainer(TContainingTarget target)
        {
            if (_getter == null)
                throw new InvalidOperationException(
                    string.Format("Unable to get value for element {0} from container {1}, no getter is available.",
                                  typeof(TTarget),
                                  typeof(TContainingTarget)));
            return _getter(target);
        }

        public void SetOnContainer(TContainingTarget target, TTarget item)
        {
            if (_setter == null)
                throw new InvalidOperationException(
                    string.Format("Unable to set value for element {0} on container {1}, no setter is available.",
                                  typeof(TTarget),
                                  typeof(TContainingTarget)));
            _setter(target, item);
        }

        public object GetFromContainer(object target)
        {
            return GetFromContainer((TContainingTarget)target);
        }

        public void SetOnContainer(object target, object item)
        {
            SetOnContainer((TContainingTarget)target, (TTarget)item);
        }
    }
}
