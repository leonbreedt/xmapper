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
    /// Represents a mapping of the XML text content of an element to an object property on a container.
    /// </summary>
    /// <typeparam name="TContainer">The type that contains the property the text content will be written to.</typeparam>
    /// <typeparam name="TProperty">The type of the property in the type.</typeparam>
    public class TextContentMapping<TContainer, TProperty> : MappingBase, ITextContentMapping
    {
        #region Fields
        readonly PropertyInfo _propertyInfo;
        readonly Func<TContainer, TProperty> _getter;
        readonly Action<TContainer, TProperty> _setter;
        readonly Func<string, TProperty> _fromXmlToPropertyValue;
        readonly Func<TProperty, string> _fromPropertyToXmlValue;
        #endregion

        /// <summary>
        /// Creates a new XML text content mapping bound to the container element.
        /// </summary>
        /// <param name="propertyExpression">A simple member expression referencing the property to associate this mapping with.</param>
        public TextContentMapping(Expression<Func<TContainer, TProperty>> propertyExpression)
            : this(null, propertyExpression, null, null)
        {
        }

        /// <summary>
        /// Creates a new XML text content mapping bound to a container element.
        /// </summary>
        /// <param name="propertyExpression">A simple member expression referencing the property to associate this mapping with.</param>
        /// <param name="customDeserializer">If not <c>null</c>, a custom function to use to deserialize the XML representation of the text content into the property type.</param>
        /// <param name="customSerializer">If not <c>null</c>, a custom function to use to serialize a property value into its XML representation.</param>
        public TextContentMapping(Expression<Func<TContainer, TProperty>> propertyExpression, Func<string, TProperty> customDeserializer, Func<TProperty, string> customSerializer)
            : this(null, propertyExpression, customDeserializer, customSerializer)
        {
        }

        /// <summary>
        /// Creates a new XML text content mapping.
        /// </summary>
        /// <param name="name">The XML name of the attribute.</param>
        /// <param name="propertyExpression">A simple member expression referencing the property to associate this mapping with.</param>
        public TextContentMapping(XName name, Expression<Func<TContainer, TProperty>> propertyExpression)
            : this(name, propertyExpression, null, null)
        {
        }

        /// <summary>
        /// Creates a new XML text content mapping.
        /// </summary>
        /// <param name="name">The XML name of the attribute.</param>
        /// <param name="propertyExpression">A simple member expression referencing the property to associate this mapping with.</param>
        /// <param name="customDeserializer">If not <c>null</c>, a custom function to use to deserialize the XML representation of the text content into the property type.</param>
        /// <param name="customSerializer">If not <c>null</c>, a custom function to use to serialize a property value into its XML representation.</param>
        public TextContentMapping(XName name, Expression<Func<TContainer, TProperty>> propertyExpression, Func<string, TProperty> customDeserializer, Func<TProperty, string> customSerializer)
            : base(typeof(TContainer), name)
        {
            _propertyInfo = ReflectionHelper.GetPropertyInfoFromExpression(propertyExpression);
            _getter = ReflectionHelper.GetTypedPropertyGetterDelegate<TContainer, TProperty>(_propertyInfo);
            _setter = ReflectionHelper.GetTypedPropertySetterDelegate<TContainer, TProperty>(_propertyInfo);
            _fromXmlToPropertyValue = customDeserializer ?? ReflectionHelper.GetXmlSimpleTypeReaderDelegate<TProperty>();
            _fromPropertyToXmlValue = customSerializer ?? ReflectionHelper.GetXmlSimpleTypeWriterDelegate<TProperty>();

            if (_fromXmlToPropertyValue == null)
                throw new ArgumentException(string.Format("Unable to determine how to deserialize property {0} of {1} from an XML representation.", _propertyInfo.Name, _propertyInfo.DeclaringType));
            if (_fromPropertyToXmlValue == null)
                throw new ArgumentException(string.Format("Unable to determine how to serialize property {0} of {1} into an XML representation.", _propertyInfo.Name, _propertyInfo.DeclaringType));
        }

        public string GetValueInXmlForm(object target)
        {
            var value = _getter((TContainer)target);
            if (value != null)
                return _fromPropertyToXmlValue(value);
            return null;
        }

        public void SetValueFromXmlForm(object target, string value)
        {
            if (value != null)
                _setter((TContainer)target, _fromXmlToPropertyValue(value));
        }

        public override IAttributeMapping[] Attributes { get { return NoAttributes; } internal set { } }
        public override IChildElementMapping[] ChildElements { get { return NoChildElements; } internal set { } }
        public override ITextContentMapping TextContent { get { return null; } internal set { } }
        public override ITextContentMapping[] ChildTextElements { get { return NoChildTextElements; } internal set { } }
    }
}
