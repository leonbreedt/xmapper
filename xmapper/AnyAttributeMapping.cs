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
using System.Xml;
using System.Xml.Linq;

namespace XMapper
{
    /// <summary>
    /// Represents a mapping of an XML custom attribute to a collection object property. That is, any XML attribute
    /// that is not explicitly declared to be supported will be added to the collection in the specified property.
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

        public object DeserializeAttribute(XmlReader reader)
        {
            return new XAttribute(XNamespace.Get(reader.NamespaceURI) + reader.LocalName,
                                  reader.Value);
        }

        public void SerializeAttribute(XmlWriter writer, object attribute)
        {
            var xAttribute = (XAttribute)attribute;
            if (!xAttribute.IsNamespaceDeclaration)
            {
                writer.WriteAttributeString(xAttribute.Name.LocalName,
                                            xAttribute.Name.NamespaceName,
                                            xAttribute.Value);
            }
        }

        public IList GetAttributes(object target)
        {
            return GetCustomNodeList(target);
        }

        public void AddToAttributes(object target, object attribute)
        {
            AddToCustomNodeList(target, (XAttribute)attribute);
        }
    }

    /// <summary>
    /// Represents a mapping of an XML any attribute to a collection object property. That is, any XML attribute
    /// that is not explicitly declared to be supported will be added to the collection in the specified property.
    /// </summary>
    /// <typeparam name="TContainer">The type that contains the property this mapping is associated with.</typeparam>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    public class AnyAttributeMapping<TContainer, TAttribute> : AnyMappingBase<TContainer, TAttribute>, IAnyAttributeMapping
    {
        #region Fields
        readonly Func<XmlReader, TAttribute> _customDeserializer;
        readonly Action<XmlWriter, TAttribute> _customSerializer;
        #endregion

        /// <summary>
        /// Creates a new XML attribute mapping for custom attributes.
        /// </summary>
        /// <param name="propertyExpression">A simple member expression referencing the property to associate this mapping with.</param>
        /// <param name="customDeserializer">Custom deserialization function.</param>
        /// <param name="customSerializer">Custom serialization function.</param>
        public AnyAttributeMapping(Expression<Func<TContainer, IList<TAttribute>>> propertyExpression,
                                   Func<XmlReader, TAttribute> customDeserializer,
                                   Action<XmlWriter, TAttribute> customSerializer)
            : base(propertyExpression)
        {
            if (customDeserializer == null)
                throw new ArgumentNullException("customDeserializer");
            if (customSerializer == null)
                throw new ArgumentNullException("customSerializer");

            _customDeserializer = customDeserializer;
            _customSerializer = customSerializer;
        }

        public object DeserializeAttribute(XmlReader reader)
        {
            return _customDeserializer(reader);
        }

        public void SerializeAttribute(XmlWriter writer, object attribute)
        {
            _customSerializer(writer, (TAttribute)attribute);
        }

        public IList GetAttributes(object target)
        {
            return GetCustomNodeList(target);
        }

        public void AddToAttributes(object target, object attribute)
        {
            AddToCustomNodeList(target, (TAttribute)attribute);
        }
    }
}